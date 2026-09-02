# Governance & AI Safety — AI Smart Hospital

**Principle: AI recommends, drafts, summarizes, predicts — humans approve.**  
No autonomous diagnosis, prescribing, or emergency triage. Every clinical AI output is a **draft** requiring clinician sign-off. A bypass of the human-approval gate is a **P0 bug**.

---

## 1. Human-Approval Gates (non-negotiable)

| Action | AI can | Human must | Enforcement |
|--------|--------|------------|-------------|
| Diagnosis / Assessment | Draft `ClinicalNote.Assessment` | **Review, edit, explicitly Sign** before saving as final | `ClinicalNoteApprovalService.Sign()` + `POST /api/clinicalnotes/{id}/sign` — only `Doctor` / `Admin` role; audit logged |
| Prescription | Draft `PrescriptionDraft` | Review & sign | Same; UI shows **AI DRAFT** badge until signed; printed Rx includes “AI draft — clinician-approved” |
| Discharge sign-off | Draft `DischargeSummary` | **Approve** before release/print | `POST /api/dischargesummaries/{id}/approve` — immutable after; edits create new version |
| Claim submission | Pre-check (flags) | Finance reviews flags, then submit | `ClaimFlag` queue with owner/ageing |

**Config:** `human approval required` actions (`diagnosis`, `prescription`, `discharge sign-off`) are hard-coded in `ClinicalNoteApprovalService` / controllers and cannot be disabled via `FeatureFlag`. Feature flags only gate *module visibility*, not safety.

---

## 2. AI Output Lifecycle & Tagging

```
Raw transcript / admission data
        ↓ IAiClient.GenerateXxxAsync (Stub or Http)
   AiOutputLog (promptTemplate, promptVersion, modelName, modelVersion, inputSummary de-identified, outputContent, taskType, confidence)
        ↓ status = Draft
   ClinicalNote / DischargeSummary { Status=AiDraft, IsAiGenerated=true, AiOutputLogId=FK, badge="AI_DRAFT" }
        ↓ clinician Review/Edit
   Sign / Approve  →  Status=Signed/Approved, SignedById, SignedAt, SignatureHash
        ↓ AiOutputLog.Status=Approved, ApprovedById/At
   Immutable — future edits → CreateAmendedVersion() → new row, PreviousVersionId, Version+1
```

- UI: every AI block rendered with **`<AiDraftBadge>`** (`amber` + label `AI DRAFT — review required`) until `Signed`.
- DB: `AiOutputLog.EntityType/EntityId` links output to versioned record.
- API: `GET /api/audit/ai-outputs` lists all for oversight.

---

## 3. Model / Prompt / Version Tracking

Table `AiOutputLog`:

| Column | Purpose |
|--------|---------|
| `PromptTemplate` | e.g. `scribe-v1`, `discharge-v1` |
| `PromptVersion` | semantic version for A/B & rollback |
| `ModelName` / `ModelVersion` | `stub-model 1.0` or `gpt-4o-mini` + provider version |
| `InputSummary` | de-identified (never raw PHI) |
| `OutputContent` | full draft |
| `TaskType` | `Scribe`, `DischargeSummary`, `Faq`, `Insight`, `ClaimCheck` |
| `Status` | `Draft` → `Approved`/`Rejected`/`Superseded` |
| `WasEdited` / `EditedDiff` | clinician correction |
| `ConfidenceScore` | if provider returns it |

Needed for: audit, reproducibility, post-incident review, acceptance-rate tuning (`>85%` target after prompt tuning).

---

## 4. Audit Logging (immutable, append-only)

- **Cross-cutting middleware:** `AuditMiddleware` + per-controller `IAuditService.LogAsync()` on every patient/financial/clinical read/write.
- **Table `AuditLogEntry`:** `Timestamp, UserId, UserName, UserRole, Action, EntityType, EntityId, Details, IpAddress, UserAgent, IsSensitive`.
- **Append-only:** no `UPDATE`/`DELETE` path in application code; retention policy field + export stub for DPDP.
- **Query:** `GET /api/audit` (Admin only), filtered by `entityType`.

---

## 5. Accuracy & Override Tracking

- **Correction rate:** `ClinicalNote.WasEdited` (via diff) + `AiOutputLog.WasEdited` measures how often clinicians edit AI drafts.
- **Rejection:** `AiOutputLog.Status=Rejected` + `RejectionReason`.
- **Dashboard:** acceptance rate computed as `Approved / (Approved+Rejected+Edited)` — target **>85%** after tuning; below triggers prompt/knowledge-base review.
- **Safety incident:** any AI draft that reaches patient-facing discharge/invoice without `SignedAt` is **P0** — detected by `SELECT … WHERE IsAiGenerated=true AND SignedAt IS NULL AND CreatedAt < NOW() - interval '...'` monitor (to be added as health check).

---

## 6. Secrets & PHI

- **Never committed:** `AI_API_KEY`, `Jwt:Key`, `ConnectionStrings`, `Snomed/ABHA` sample data uses fictional names (`example.test`).
- **Via env / secrets manager:** `Environment.GetEnvironmentVariable("AI_API_KEY")` / `IConfiguration["Jwt:Key"]`; `HttpAiClient` falls back to stub if missing.
- **DPDP-aligned:** data minimization (only fields needed for task), purpose field on `ConsentRecord`, retention policy, subject access/export stub endpoint (`GET /api/patients/{id}/export` — to be implemented).
- **Consent:** `ConsentRecord` (`Purpose: Treatment/Billing/Research/Insurance`, `Status`, `GrantedAt/ExpiresAt/RevokedAt`, `HipId` for ABDM). Middleware stub checks consent before clinical read (full enforcement before prod).

---

## 7. RBAC Matrix (server-enforced)

| Resource | Admin | Doctor | Nurse | FrontDesk | Billing | Pharmacy | Management | LabTech |
|----------|-------|--------|-------|-----------|---------|----------|------------|---------|
| Patients CRUD | R | R/W | R | R/W | R | — | R | — |
| Appointments | R/W | R/W | R | R/W | — | — | R | — |
| ClinicalNotes draft | R/W | R/W | R/W | — | — | — | — | — |
| ClinicalNotes sign | R/W | R/W | — | — | — | — | — | — |
| Discharge approve | R/W | R/W | R | — | — | — | — | — |
| Revenue/Claims | R/W | R | — | — | R/W | — | R | — |
| Pharmacy | R/W | R | — | — | — | R/W | R | — |
| Lab | R/W | R/W | R | — | — | — | — | R/W |
| Beds | R/W | R | R/W | R/W | — | — | R | — |
| Audit/AI logs | R/W | — | — | — | — | — | — | — |
| FeatureFlags toggle | R/W | — | — | — | — | — | — | — |

Never trust client role — checked via `[Authorize(Roles=...)]` + service-level checks.

---

## 8. AI Provider Swapping

- Code behind `IAiClient` — business logic never calls provider SDK directly.
- `StubAiClient` (deterministic, safe for CI) → `HttpAiClient` (OpenAI/Azure/Claude) via `AI_*` env + DI in `Program.cs`:
  ```csharp
  if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AI_API_KEY")))
      services.AddHttpClient<IAiClient, HttpAiClient>();
  else
      services.AddSingleton<IAiClient, StubAiClient>();
  ```
- `ISttProvider` similarly pluggable (stub → cloud STT).

---

## 9. Incident Response

1. **Report:** audit log + `AiOutputLog` + `ClinicalNote` version history give full chain.
2. **Mitigate:** disable module via `FeatureFlag` (`module.scribe` etc.) without code deploy.
3. **Review:** prompt/version/model compared across incidents; hotfix prompt version.
4. **Prevent:** add regression test in `ClinicalNoteApprovalTests` documenting invariant (e.g., `Human_Approval_Gate_Enforced_Critical_Policy`).

---

## 10. Compliance Posture (India)

- **ABDM/FHIR:** `Patient`/`Encounter`/`LabResult` shapes compatible (identifier, code, value, reference range); mappable to FHIR R4 resources.
- **Coding:** `SnomedCode`, `LoincCode`, `Icd10Code` nullable on relevant entities — ready for mapping without rewrite.
- **No certification claim:** banners/footers state “decision-support drafts pending clinician sign-off, not certified”.

---

*Reviewed by: Hospital compliance stakeholders • Last updated: 2026-09 • Next review: after prompt tuning (measure acceptance rate).*
