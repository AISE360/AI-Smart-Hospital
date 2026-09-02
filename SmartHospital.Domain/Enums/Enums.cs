namespace SmartHospital.Domain.Enums;

public enum Gender { Male, Female, Other, Unknown }
public enum EncounterType { OPD, IPD, Emergency, Teleconsult }
public enum EncounterStatus { Planned, InProgress, Finished, Cancelled }
public enum AppointmentStatus { Scheduled, CheckedIn, InConsultation, Completed, Cancelled, NoShow }
public enum BedStatus { Available, Occupied, Cleaning, Blocked, Maintenance }
public enum AdmissionStatus { Admitted, Discharged, Transferred, Cancelled }
public enum ClinicalNoteStatus { Draft, AiDraft, PendingReview, Signed, Amended }
public enum DischargeSummaryStatus { Draft, AiDraft, PendingApproval, Approved, Amended }
public enum InvoiceStatus { Draft, Finalized, Paid, PartiallyPaid, Cancelled }
public enum ClaimStatus { NotSubmitted, Submitted, UnderReview, Approved, Rejected, PartiallyApproved, Resubmitted }
public enum ClaimFlagType { MissingDocument, CodingMismatch, PayerSpecific, DuplicateCharge, PackageException, MissingDocumentation }
public enum ClaimFlagStatus { Open, Assigned, Resolved, Dismissed }
public enum LabOrderStatus { Ordered, Collected, InProgress, Completed, Cancelled }
public enum LabResultCriticality { Normal, Abnormal, Critical, Panic }
public enum AiOutputStatus { Draft, Approved, Rejected, Superseded }
public enum StaffRole
{
    Admin,
    Doctor,
    Nurse,
    FrontDesk,
    Billing,
    Pharmacy,
    Management,
    LabTechnician
}
public enum ConsentStatus { Granted, Revoked, Expired }
public enum KpiCategory { Bed, Opd, Revenue, Pharmacy, Lab, Claims, Operations }
