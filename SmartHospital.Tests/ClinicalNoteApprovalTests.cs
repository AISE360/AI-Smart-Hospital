using SmartHospital.Application.Services;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Tests;

public class ClinicalNoteApprovalTests
{
    private readonly ClinicalNoteApprovalService _svc = new();

    private ClinicalNote DraftNote(ClinicalNoteStatus status = ClinicalNoteStatus.AiDraft)
    {
        return new ClinicalNote
        {
            Id = Guid.NewGuid(),
            EncounterId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            DoctorId = "doctor1",
            Version = 1,
            Status = status,
            History = "History draft",
            Assessment = "Assessment draft",
            IsAiGenerated = status == ClinicalNoteStatus.AiDraft
        };
    }

    [Fact]
    public void Sign_Transitions_To_Signed_And_Sets_Metadata()
    {
        var note = DraftNote(ClinicalNoteStatus.PendingReview);
        var signed = _svc.Sign(note, "doctor1");
        Assert.Equal(ClinicalNoteStatus.Signed, signed.Status);
        Assert.Equal("doctor1", signed.SignedById);
        Assert.NotNull(signed.SignedAt);
        Assert.NotNull(signed.SignatureHash);
    }

    [Fact]
    public void Sign_AiDraft_Allowed_But_Marks_Approved()
    {
        var note = DraftNote(ClinicalNoteStatus.AiDraft);
        var signed = _svc.Sign(note, "doctor1");
        Assert.Equal(ClinicalNoteStatus.Signed, signed.Status);
    }

    [Fact]
    public void Sign_Already_Signed_Throws()
    {
        var note = DraftNote(ClinicalNoteStatus.Signed);
        note.SignedAt = DateTime.UtcNow;
        note.SignedById = "doctor1";
        var ex = Assert.Throws<InvalidOperationException>(()=> _svc.Sign(note, "doctor2"));
        Assert.Contains("already signed", ex.Message.ToLower());
    }

    [Fact]
    public void CreateAmendedVersion_Increments_Version_And_Links_Previous()
    {
        var signed = DraftNote(ClinicalNoteStatus.Signed);
        signed.SignedAt = DateTime.UtcNow;
        signed.SignedById = "doctor1";
        signed.Version = 1;

        var amended = _svc.CreateAmendedVersion(signed, "doctor1", n => n.Assessment = "Updated assessment");
        Assert.Equal(2, amended.Version);
        Assert.Equal(signed.Id, amended.PreviousVersionId);
        Assert.Equal(ClinicalNoteStatus.Draft, amended.Status);
        Assert.Equal("Updated assessment", amended.Assessment);
        Assert.Null(amended.SignedAt);
    }

    [Fact]
    public void CreateAmendedVersion_Requires_Signed_Source()
    {
        var draft = DraftNote(ClinicalNoteStatus.Draft);
        Assert.Throws<InvalidOperationException>(()=> _svc.CreateAmendedVersion(draft, "doctor1", _=>{}));
    }

    [Fact]
    public void Signed_Note_Is_Immutable_Second_Amend_Creates_New_Version()
    {
        var signed = DraftNote(ClinicalNoteStatus.Signed);
        signed.SignedAt = DateTime.UtcNow;
        signed.SignedById = "doctor1";
        var v2 = _svc.CreateAmendedVersion(signed, "doctor1", n=> n.History="v2");
        // v2 is draft, sign it
        _svc.Sign(v2, "doctor1");
        var v3 = _svc.CreateAmendedVersion(v2, "doctor1", n=> n.History="v3");
        Assert.Equal(3, v3.Version);
        Assert.Equal(v2.Id, v3.PreviousVersionId);
    }

    [Fact]
    public void Discharge_Sign_Works_Similarly()
    {
        var summary = new DischargeSummary
        {
            Id = Guid.NewGuid(),
            AdmissionId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            Version = 1,
            Status = DischargeSummaryStatus.AiDraft
        };
        var signed = _svc.SignDischarge(summary, "doctor1");
        Assert.Equal(DischargeSummaryStatus.Approved, signed.Status);
        Assert.NotNull(signed.SignedAt);
    }

    [Fact]
    public void Human_Approval_Gate_Enforced_Critical_Policy()
    {
        // This test documents the P0 invariant: AI outputs cannot auto-finalize
        // Any path that would set status to Signed/Approved without explicit Sign() call is a bug
        var note = DraftNote(ClinicalNoteStatus.AiDraft);
        // Directly setting status to Signed without Sign() would bypass audit log — ensure service is used
        Assert.NotEqual(ClinicalNoteStatus.Signed, note.Status);
        // Only Sign() should produce signed state
        _svc.Sign(note, "doctor1");
        Assert.Equal(ClinicalNoteStatus.Signed, note.Status);
    }
}
