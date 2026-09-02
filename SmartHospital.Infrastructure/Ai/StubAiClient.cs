using SmartHospital.Application.Interfaces;

namespace SmartHospital.Infrastructure.Ai;

/// <summary>
/// Deterministic stub for local dev without API keys. Mirrors IAiClient contract.
/// Replace via HttpAiClient when AI_API_KEY is configured.
/// </summary>
public class StubAiClient : IAiClient
{
    public Task<AiCompletionResult> CompleteAsync(AiCompletionRequest request, CancellationToken ct = default)
    {
        var content = $"[STUB {request.TaskType} v{request.PromptVersion}] Generated content for prompt: {request.Prompt.Substring(0, Math.Min(120, request.Prompt.Length))}...";
        return Task.FromResult(new AiCompletionResult(content, "stub-model", "1.0", 0.92, 128));
    }

    public Task<string> GenerateFaqAnswerAsync(string question, string knowledgeBase, CancellationToken ct = default)
    {
        // Simple KB lookup simulation
        var lower = question.ToLowerInvariant();
        string answer;
        if (lower.Contains("opd") || lower.Contains("timing"))
            answer = "OPD timings are 9:00 AM–5:00 PM, Monday–Saturday. Token issued at reception. Emergency is 24x7.";
        else if (lower.Contains("appointment") || lower.Contains("book"))
            answer = "You can book via front desk, phone, or patient portal. Please provide MRN or phone number.";
        else if (lower.Contains("insurance") || lower.Contains("tpa"))
            answer = "We accept major TPAs. Please carry insurance card, photo ID, and pre-authorization form at admission.";
        else if (lower.Contains("visit") || lower.Contains("discharge"))
            answer = "Visiting hours: 4–7 PM. Discharge summaries are issued after consultant approval, usually within 2 hours of clearance.";
        else
            answer = $"Thank you for your query: '{question}'. Our front desk will assist you shortly. For medical advice, please consult your doctor directly.";
        return Task.FromResult(answer);
    }

    public Task<ClinicalNoteAiResult> GenerateClinicalNoteAsync(ClinicalNoteAiRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new ClinicalNoteAiResult(
            History: $"Patient presents with: {request.TranscriptOrNotes.Truncate(180)} [AI DRAFT — review required]",
            Examination: "Vitals: BP 122/80, HR 78, Temp 98.6F, SpO2 98%. General examination unremarkable. Systemic exam as per department protocol.",
            Assessment: "Assessment pending clinician review. Correlate clinically. No autonomous diagnosis.",
            InvestigationOrders: "As per clinical indication: CBC, RFT, ECG if warranted.",
            PrescriptionDraft: "Rx draft — requires clinician approval. No prescription finalized by AI.",
            FollowUp: "Follow-up in 7 days or SOS. Red-flag counselling done.",
            RawOutput: $"[AI_DRAFT ClinicalNote model=stub v1] Input: {request.TranscriptOrNotes.Truncate(80)}"
        ));
    }

    public Task<DischargeSummaryAiResult> GenerateDischargeSummaryAsync(DischargeSummaryAiRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new DischargeSummaryAiResult(
            AdmissionCourse: request.AdmissionCourse.Truncate(400) + " [AI DRAFT]",
            Investigations: request.InvestigationsJson.Truncate(300),
            Procedures: request.ProceduresJson.Truncate(300),
            TreatmentGiven: "Supportive care, medications as charted. Details per case sheet.",
            ConditionAtDischarge: "Stable at discharge. Vitals normal. Afebrile.",
            DischargeAdvice: "Medications as prescribed. Diet as advised. Rest, hydration, wound care if applicable.",
            FollowUpPlan: "OPD follow-up in 7 days with reports. SOS if fever, chest pain, breathlessness, or wound issues.",
            FullContent: $"DISCHARGE SUMMARY [AI DRAFT - requires consultant sign-off]\nCourse: {request.AdmissionCourse.Truncate(200)}\nInvestigations: {request.InvestigationsJson.Truncate(100)}\nAdvice: Follow prescribed medications and follow-up."
        ));
    }

    public Task<string> GenerateDailyInsightAsync(DailyInsightRequest request, CancellationToken ct = default)
    {
        if (!request.Deltas.Any())
            return Task.FromResult("No significant change in KPIs. Operations stable.");

        var top = request.Deltas.OrderByDescending(d => Math.Abs(d.DeltaPercent)).Take(3)
            .Select(d => $"{d.Metric}: {d.Current} (Δ {d.DeltaPercent:+0.0;-0.0}%)");
        var insight = $"**What changed?** {string.Join(", ", top)}. **Why?** Based on seeded trends, variance within normal range. **Attention:** Review flagged leakage and expiry alerts on dashboard.";
        return Task.FromResult(insight);
    }

    public Task<ClaimPreCheckResult> PreCheckClaimAsync(ClaimPreCheckRequest request, CancellationToken ct = default)
    {
        var issues = new List<string>();
        var warnings = new List<string>();
        if (string.IsNullOrWhiteSpace(request.Icd10Code))
            issues.Add("Missing ICD-10 code");
        if (request.ClaimedAmount <= 0)
            issues.Add("Claimed amount must be > 0");
        if (!request.AttachedDocuments.Contains("DischargeSummary"))
            issues.Add("Missing document: DischargeSummary");
        if (!request.AttachedDocuments.Contains("Invoice"))
            issues.Add("Missing document: Invoice");
        if (request.PayerName.Contains("Star", StringComparison.OrdinalIgnoreCase) && !request.AttachedDocuments.Contains("PreAuth"))
            warnings.Add("Star Health requires PreAuth document");
        var passed = issues.Count == 0;
        return Task.FromResult(new ClaimPreCheckResult(passed, issues, warnings, passed ? 0.88 : 0.45));
    }
}

internal static class StringExt
{
    public static string Truncate(this string s, int max) =>
        s.Length <= max ? s : s.Substring(0, max) + "…";
}
