# AI Smart Hospital — 50-bed Hospital OS (ASP.NET Core 8 + Electron)

> **AI layer that sits on top of HMIS/EMR, billing, LIS, RIS/PACS, pharmacy** — it does **not** replace them.  
> Every clinical AI output is an **AI_DRAFT** requiring explicit clinician sign-off.

Stack: **ASP.NET Core 8 Web API (Clean Architecture)** + **PostgreSQL (EF Core)** + **React + TypeScript + Vite + Tailwind** inside **Electron** + **SignalR** live KPIs + **IAiClient** pluggable LLM.

---

## 1. Architecture

```
[Electron Desktop (React/TS)] <--HTTPS/JWT/SignalR--> [ASP.NET Core Web API :5000]
  - Role-based screens (Reception, Doctor, Billing, Pharmacy, Admin, Command Center)
  - Offline-tolerant (queues writes, shows cached)
                                            |
                     [PostgreSQL (or InMemory dev)] [AuditLog*] [AiOutputLog]
                                            |         *append-only, immutable
                                     IAiClient (Stub ↔ OpenAI/Azure/Claude via env)
                                     ISttProvider, Integration Adapters (HMIS/LIS/Billing)
```

**Clean Architecture:** `Domain → Application → Infrastructure → Api` • RBAC via Identity + JWT • MFA for Admin/Billing • Audit middleware (action filter) • Serilog • Swagger • Health checks • CORS • Rate limit ready.

**Electron:** `electron/main.ts` (BrowserWindow) + `preload.ts` (contextBridge) • Vite renderer on `http://localhost:5173` (dev) or `dist/index.html` (prod) • `electron-builder` for installers • `electron-updater` hooks.

---

## 2. Modules (built in order, per spec)

| # | Module | Key endpoints / screens |
|---|--------|--------------------------|
| **0** | **Foundation** | `Identity` roles: Admin, Doctor, Nurse, FrontDesk, Billing, Pharmacy, Management, LabTechnician • `Department`/`Ward`/`Bed` • `AuditLogEntry` (immutable) • `FeatureFlag` per module • RBAC enforced server-side |
| **1** | **AI Receptionist & Appointments** | `Patients` search/registration (ABDM/ABHA ready) • `Appointments` booking + `availability` • `FAQ` (`IAiClient.GenerateFaqAnswerAsync` + KB, no hallucination) • reminders job stub |
| **2** | **AI Medical Scribe** | `ClinicalNotes/draft` → `generate-ai` (IAiClient) → structured SOAP draft • **Sign** (immutable, versioned) • `Amended` creates new version |
| **3** | **AI Discharge Summary** | Pulls admission `ServiceOrders` + `LabOrders` → `generate` → draft → **Approve** (same signing pattern) |
| **4** | **Revenue AI (flagship)** | `RevenueReconciliationService` compares `ServiceOrders` vs `InvoiceLines` → leakage queue `{recoverableRevenue, count, byCategory}` • **₹ recovered/month** prominent on dashboard |
| **5** | **Claim Pre-check & Denial Analytics** | `Claims/precheck` via `IAiClient` (missing docs, ICD mismatch, payer rules) • `ClaimFlag` queue • `denials/analytics` (by payer/dept/reason, trend) |
| **6** | **Pharmacy AI** | `PharmacyItem`/`StockLevel`/`ExpiryBatch` • demand forecast (`MovingAverageForecastService` behind `IPharmacyForecastService`, swappable ML) • expiry-risk (90d) • stock-out prediction |
| **7** | **Lab & Bed/Ops** | `LabOrders` + `LabResult` (critical routing, TAT) • `Beds/occupancy` (forecast expected discharges, ward breakdown) • OT schedule stub |
| **8** | **Command Center** | `Dashboard/kpis` (beds, OPD, LOS, revenue/day, claims, leakage, expiry) • `Dashboard/insight` (daily natural-language “What changed? Why? Attention?” via `IAiClient` on aggregated deltas) • **SignalR** `DashboardHub` live push |
| **9** | **Governance & AI Safety** | `AiOutputLog` (prompt/model/version/output/approvedBy) • `AI_DRAFT` badge until sign • Config `human approval required` for diagnosis/rx/discharge • Override tracking for acceptance rate • `GOVERNANCE.md` |

**India compliance:** FHIR-shaped `Patient/Encounter/Observation` fields • nullable `SnomedCode/LoincCode/Icd10Code` • `ConsentRecord` + middleware • DPDP fields (purpose limitation, retention, export stub) • no certification claims in UI.

---

## 3. Data Model (EF Core)

`Patient, Encounter, Appointment, Ward, Bed, Admission, ServiceOrder, ClinicalNote (versioned, signed), DischargeSummary (versioned, signed), Invoice, InvoiceLine, Claim, ClaimFlag, DenialRecord, PharmacyItem, StockLevel, ExpiryBatch, LabOrder, LabResult, StaffUser (Identity), Department, AuditLogEntry, AiOutputLog, KpiSnapshot, ConsentRecord, FeatureFlag`

- EF Core migrations from day one (`ApplicationDbContext`).
- Decimals with precision 18.
- PII fields separated (e.g., `AadhaarHash` hashed, not plain).

---

## 4. Prerequisites

- **.NET SDK 8** (or 10 with `DOTNET_ROLL_FORWARD=Major`) — `dotnet --version` ≥ 8.0
- **Node 18+** — `node --version`
- **PostgreSQL 14+** *optional* — dev defaults to **InMemory** (`UseInMemoryDatabase: true`). Set `false` + `ConnectionStrings:DefaultConnection` for Postgres.
- **Git**, **npm**

> **Test host note:** If only .NET 10 SDK is installed (no `Microsoft.AspNetCore.App 8`), run tests with `DOTNET_ROLL_FORWARD=Major` or install AspNetCore 8 runtime: see “Troubleshooting”.

---

## 5. Running locally

### Backend API (http://localhost:5000, Swagger at `/swagger`)

```bash
cd SmartHospital.Api
# env - never hardcode AI keys
# InMemory demo (no Postgres needed)
dotnet run --urls http://localhost:5000

# With Postgres
# appsettings.json:  "UseInMemoryDatabase": false
#                    "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5432;Database=smarthospital;Username=postgres;Password=postgres"
# then:
# dotnet ef database update
# dotnet run

# AI provider (optional - defaults to deterministic StubAiClient)
# export AI_API_KEY=sk-...
# export AI_MODEL=gpt-4o-mini
# export AI_ENDPOINT=https://api.openai.com/v1/chat/completions
# export AI_PROVIDER=openai

# JWT (change in production)
# appsettings.json Jwt:Key must be 32+ chars
```

Seed data auto-creates on first run (50 beds ~75% occupied, 40 patients, encounters, invoices with 30% leakage, claims, pharmacy, labs, KPIs, users below).

**Demo logins** (`POST /api/auth/login` or Electron login):

| username | password | role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| doctor1 | Doctor@123 | Doctor |
| doctor2 | Doctor@123 | Doctor |
| frontdesk | Front@123 | FrontDesk |
| billing | Bill@123 | Billing (MFA demo: 123456) |
| pharmacy | Pharm@123 | Pharmacy |
| nurse1 | Nurse@123 | Nurse |
| management | Manage@123 | Management |
| labtech | Lab@123 | LabTechnician |

MFA: Admin/Billing/Export require MFA (`123456` in stub). Change via `StaffUser.MfaEnabled`.

### Electron desktop client (wraps React)

```bash
cd electron-app
npm install
# dev: starts Vite + Electron together
npm run dev:electron
# or separately:
npm run dev              # Vite only at http://localhost:5173 (browser)
# in another shell:
npm run electron         # Electron window (loads http://localhost:5173 in dev)

# production
npm run build            # tsc + vite build + tsc electron
npm run build:electron   # + electron-builder installers (release/ folder)
```

Env: `VITE_API_URL` (default `http://localhost:5000`) — or preload `window.hospital.getApiUrl()`.

### Quick sanity check (after backend is up)

```bash
# login + fetch leaks (flagship metric)
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"username":"billing","password":"Bill@123"}'
# -> token
curl http://localhost:5000/api/revenue/summary -H "Authorization: Bearer <token>"
curl http://localhost:5000/health
```

---

## 6. Project layout

```
SmartHospital.Domain/          # Entities, Enums, BaseEntity
SmartHospital.Application/     # IAiClient, ISttProvider, DTOs, Services (Revenue, Approval, Forecast)
SmartHospital.Infrastructure/  # ApplicationDbContext, Ai/StubAiClient + HttpAiClient, AuditService, SeedData
SmartHospital.Api/             # Controllers (0-9), Hubs/DashboardHub, Middleware/AuditMiddleware, Services/JwtTokenService, Program.cs
SmartHospital.Tests/           # xUnit: RevenueReconciliationTests + ClinicalNoteApprovalTests + PharmacyForecastTests
electron-app/
  src/
    lib/api.ts, lib/auth.tsx
    components/Layout, KpiCard, Badge
    pages/Login, Dashboard, Patients, Appointments, Scribe, Discharge, Revenue, Claims, Pharmacy, Lab, Beds, Admin
  electron/main.ts, preload.ts
  vite.config.ts, tailwind.config.js, tsconfig.electron.json
```

---

## 7. AI abstraction

```csharp
public interface IAiClient {
  Task<AiCompletionResult> CompleteAsync(AiCompletionRequest req);
  Task<string> GenerateFaqAnswerAsync(string q, string kb);
  Task<ClinicalNoteAiResult> GenerateClinicalNoteAsync(ClinicalNoteAiRequest req);
  Task<DischargeSummaryAiResult> GenerateDischargeSummaryAsync(DischargeSummaryAiRequest req);
  Task<string> GenerateDailyInsightAsync(DailyInsightRequest req);
  Task<ClaimPreCheckResult> PreCheckClaimAsync(ClaimPreCheckRequest req);
}
```

- **StubAiClient** — deterministic, no network, safe for CI/demo.
- **HttpAiClient** — OpenAI-compatible HTTP, reads `AI_API_KEY / AI_MODEL / AI_ENDPOINT / AI_PROVIDER` from env, falls back to stub if unset.
- **Never hardcoded**: keys via `Environment.GetEnvironmentVariable` / `IConfiguration` + secrets manager in prod.
- **Prompt/version/model tracking**: every call logs `AiOutputLog` (`promptTemplate, promptVersion, modelName, modelVersion, inputSummary (de-identified), outputContent`).

Swap provider without touching business logic: register `HttpAiClient` as `IAiClient` in `Program.cs`.

---

## 8. Security & Governance

- **RBAC server-side only** — every controller has `[Authorize(Roles=...)]`, client role ignored.
- **MFA** for Admin, Finance, export.
- **TLS** (Electron ↔ API HTTPS), **encryption at rest** (Postgres TDE in prod).
- **Immutable audit log** (`AuditLogEntry` append-only, `AuditMiddleware` logs sensitive reads/writes).
- **Rate limiting** + **input validation** + **OWASP** (EF parameterized queries, XSS headers, etc.).
- **Human approval gates** — see `GOVERNANCE.md` — diagnosis/prescription/discharge can never auto-finalize.

---

## 9. Testing

```bash
# Backend unit tests (Revenue AI + approval workflow — highest-risk pieces)
dotnet test SmartHospital.Tests/SmartHospital.Tests.csproj
# with .NET 10 SDK only:
DOTNET_ROLL_FORWARD=Major dotnet test SmartHospital.Tests/SmartHospital.Tests.csproj

# Frontend (Vitest/Playwright ready — add tests in electron-app)
cd electron-app
npm run build   # type-check + production build verification
```

See `SmartHospital.Tests/RevenueReconciliationTests.cs` (leak detection, duplicate, recoverable sum, 320-admission simulation) and `ClinicalNoteApprovalTests.cs` (sign immutability, amended version, P0 invariant).

---

## 10. Environment variables

| Var | Purpose | Default |
|-----|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` | `Development` |
| `ConnectionStrings__DefaultConnection` | Postgres connection | `Host=localhost...` |
| `Jwt__Key` | 32+ char HMAC secret | dev-only key |
| `UseInMemoryDatabase` | `true` = InMemory, `false` = Npgsql | `true` |
| `AI_API_KEY` | AI provider key (stub if empty) | *(none)* |
| `AI_MODEL` | e.g. `gpt-4o-mini` | `gpt-4o-mini` |
| `AI_ENDPOINT` | OpenAI-compatible endpoint | `https://api.openai.com/v1/chat/completions` |
| `VITE_API_URL` | Frontend API base | `http://localhost:5000` |

Never commit secrets — see `appsettings.json` + user secrets.

---

## 11. Docker & CI

```dockerfile
# Backend Dockerfile (example)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
COPY SmartHospital.Api/bin/Release/net8.0/publish/ .
ENTRYPOINT ["dotnet", "SmartHospital.Api.dll"]
```

Build: `dotnet publish SmartHospital.Api/SmartHospital.Api.csproj -c Release`

CI skeleton: `.github/workflows/ci.yml` (dotnet build + test + npm build + docker build + electron-builder).

---

## 12. Roadmap vs Build

- **Weeks 1–2 (Foundation):** Module 0 + auth/RBAC + Electron→API + CI + seed
- **Weeks 3–6 (MVP):** Modules 1,2,3,4 (basic), 8 (basic)
- **Weeks 7–12:** Modules 5,6,7, full Module 4 SignalR, Module 9 wired
- **Months 4–6 / 7–12:** forecasting deepening, denial RCA, readmission/LOS, imaging adapter (validated, off by default)

**Success scorecard (6-mo targets, seeded data):** OPD wait −10–20%, self-service 40–60%, doc time −20–30%, rejection −10–20%, leakage ₹1–2L/mo flagged, expiry −10–20%, acceptance >85%, **zero** AI-safety bypass.

---

## 13. Troubleshooting

- **Tests: `Microsoft.AspNetCore.App 8.0.0 missing`** → install AspNetCore 8 runtime *or* `DOTNET_ROLL_FORWARD=Major dotnet test`
- **API 401:** token expired (8h) → re-login; check `Jwt__Key` matches between token gen and validation.
- **InMemory vs Postgres:** InMemory does not enforce unique indexes across restarts — Postgres does.
- **SignalR 401:** ensure `Authorization: Bearer <token>` *and* query `?access_token=` for WebSocket.
- **Electron blank:** Vite not running — `wait-on http://localhost:5173` ensures order.

---

## 14. Disclaimer

Illustrative 50-bed hospital in India, fictional seed data (no real PHI). AI clinical outputs are **decision-support drafts**; not certified; require validation before clinical/regulatory use. Labelled clearly in code/UI.

---

*Built for maintainability: boring, modular, auditable — hospital infrastructure.*
