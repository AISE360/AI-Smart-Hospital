namespace SmartHospital.Application.Dtos;

public record PatientDto(Guid Id, string Mrn, string FullName, string Gender, DateTime DateOfBirth, string? Phone, string? Email);
public record CreatePatientRequest(string FullName, string Gender, DateTime DateOfBirth, string? Phone, string? Email, string? Address, string? AbhaId);
public record AppointmentDto(Guid Id, Guid PatientId, string PatientName, string DoctorId, string DoctorName, DateTime ScheduledAt, string Status, string? TokenNumber);
public record CreateAppointmentRequest(Guid PatientId, string DoctorId, DateTime ScheduledAt, string? Reason);
public record ClinicalNoteDto(Guid Id, Guid EncounterId, Guid PatientId, int Version, string Status, string? History, string? Assessment, bool IsAiGenerated, string? SignedBy, DateTime? SignedAt);
public record CreateClinicalNoteRequest(Guid EncounterId, Guid PatientId, string? RawTranscript, string? History, string? Examination, string? Assessment, string? InvestigationOrders, string? PrescriptionDraft, string? FollowUp);
public record DischargeSummaryDto(Guid Id, Guid AdmissionId, Guid PatientId, int Version, string Status, string? FullContent, bool IsAiGenerated, string? SignedBy, DateTime? SignedAt);
public record InvoiceDto(Guid Id, string InvoiceNumber, Guid PatientId, decimal TotalAmount, string Status, DateTime InvoiceDate, List<InvoiceLineDto> Lines);
public record InvoiceLineDto(string ServiceCode, string Description, decimal UnitPrice, int Quantity, decimal LineTotal);
public record RevenueLeakDto(Guid ServiceOrderId, string ServiceCode, string ServiceName, decimal UnitPrice, int Quantity, decimal LeakageAmount, string Reason, Guid EncounterId, string PatientName);
public record ClaimDto(Guid Id, string ClaimNumber, Guid InvoiceId, string PayerName, decimal ClaimedAmount, decimal? ApprovedAmount, string Status);
public record PharmacyItemDto(Guid Id, string Code, string Name, string Category, decimal Mrp, int QuantityOnHand, int? DaysOfStock, DateTime? PredictedStockOut);
public record ExpiryAlertDto(Guid BatchId, string ItemName, string BatchNumber, int Quantity, DateTime ExpiryDate, int DaysToExpiry, decimal ValueAtRisk);
public record LabOrderDto(Guid Id, Guid PatientId, string TestName, string Status, DateTime OrderedAt, string? Priority, string? ResultValue, string? Criticality);
public record KpiDto(string MetricName, decimal Value, string Unit, decimal? DeltaPercent, string Category);
public record DashboardDto(
    int BedsOccupied, int BedsTotal, double OccupancyPercent,
    int OpdToday, int AdmissionsToday, int DischargesToday,
    double AvgLos, decimal RevenueToday, decimal OutstandingClaims,
    double RejectionRate, int LeakageAlerts, int ExpiryAlerts,
    List<KpiDto> Kpis, string? AiInsight);
public record AuthRequest(string Username, string Password, string? MfaCode);
public record AuthResponse(string Token, string Username, string Role, string FullName, DateTime ExpiresAt);
public record FaqRequest(string Question);
public record FaqResponse(string Answer, string ModelUsed);
public record BedDto(Guid Id, string BedNumber, string WardName, string Status, string? PatientName);
public record DepartmentDto(Guid Id, string Name, string Code);
