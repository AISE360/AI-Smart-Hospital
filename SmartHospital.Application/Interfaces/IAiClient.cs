namespace SmartHospital.Application.Interfaces;

/// <summary>
/// Provider-agnostic AI abstraction. All AI calls go through this interface
/// so provider can be swapped via configuration without touching business logic.
/// </summary>
public interface IAiClient
{
    Task<AiCompletionResult> CompleteAsync(AiCompletionRequest request, CancellationToken ct = default);
    Task<string> GenerateFaqAnswerAsync(string question, string knowledgeBase, CancellationToken ct = default);
    Task<ClinicalNoteAiResult> GenerateClinicalNoteAsync(ClinicalNoteAiRequest request, CancellationToken ct = default);
    Task<DischargeSummaryAiResult> GenerateDischargeSummaryAsync(DischargeSummaryAiRequest request, CancellationToken ct = default);
    Task<string> GenerateDailyInsightAsync(DailyInsightRequest request, CancellationToken ct = default);
    Task<ClaimPreCheckResult> PreCheckClaimAsync(ClaimPreCheckRequest request, CancellationToken ct = default);
}

public record AiCompletionRequest(string Prompt, string PromptVersion, string TaskType, double Temperature = 0.2, int MaxTokens = 2000);
public record AiCompletionResult(string Content, string ModelName, string ModelVersion, double? Confidence, int TokensUsed);

public record ClinicalNoteAiRequest(string TranscriptOrNotes, string PatientContext, string Department);
public record ClinicalNoteAiResult(string History, string Examination, string Assessment, string InvestigationOrders, string PrescriptionDraft, string FollowUp, string RawOutput);

public record DischargeSummaryAiRequest(string AdmissionCourse, string InvestigationsJson, string ProceduresJson, string PatientContext);
public record DischargeSummaryAiResult(string AdmissionCourse, string Investigations, string Procedures, string TreatmentGiven, string ConditionAtDischarge, string DischargeAdvice, string FollowUpPlan, string FullContent);

public record DailyInsightRequest(IReadOnlyList<KpiDelta> Deltas, string HospitalContext);
public record KpiDelta(string Metric, decimal Current, decimal Previous, decimal DeltaPercent);

public record ClaimPreCheckRequest(string PayerName, decimal ClaimedAmount, string? Icd10Code, string? ProcedureCode, IReadOnlyList<string> AttachedDocuments);
public record ClaimPreCheckResult(bool Passed, IReadOnlyList<string> Issues, IReadOnlyList<string> Warnings, double Confidence);
