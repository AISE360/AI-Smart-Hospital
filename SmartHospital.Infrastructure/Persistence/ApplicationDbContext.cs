using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Entities;

namespace SmartHospital.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<StaffUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();
    public DbSet<DischargeSummary> DischargeSummaries => Set<DischargeSummary>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimFlag> ClaimFlags => Set<ClaimFlag>();
    public DbSet<DenialRecord> DenialRecords => Set<DenialRecord>();
    public DbSet<PharmacyItem> PharmacyItems => Set<PharmacyItem>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<ExpiryBatch> ExpiryBatches => Set<ExpiryBatch>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<AiOutputLog> AiOutputLogs => Set<AiOutputLog>();
    public DbSet<KpiSnapshot> KpiSnapshots => Set<KpiSnapshot>();
    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Decimal precision
        foreach (var entity in b.Model.GetEntityTypes())
        {
            foreach (var prop in entity.GetProperties().Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                prop.SetPrecision(18);
        }

        b.Entity<Patient>(e =>
        {
            e.HasIndex(p => p.Mrn).IsUnique();
            e.Property(p => p.FullName).HasMaxLength(200).IsRequired();
        });
        b.Entity<Department>(e => e.HasIndex(d => d.Code).IsUnique());
        b.Entity<Ward>(e => e.HasIndex(w => w.Code).IsUnique());
        b.Entity<Bed>(e => e.HasIndex(be => be.BedNumber).IsUnique());
        b.Entity<Invoice>(e => e.HasIndex(i => i.InvoiceNumber).IsUnique());
        b.Entity<Claim>(e => e.HasIndex(c => c.ClaimNumber).IsUnique());
        b.Entity<PharmacyItem>(e => e.HasIndex(p => p.Code).IsUnique());
        b.Entity<FeatureFlag>(e => e.HasIndex(f => f.Key).IsUnique());

        // Foreign keys + delete behavior
        b.Entity<Encounter>().HasOne(e => e.Patient).WithMany(p => p.Encounters).HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Admission>().HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ClinicalNote>().HasOne(c => c.PreviousVersion).WithMany().HasForeignKey(c => c.PreviousVersionId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<DischargeSummary>().HasOne(d => d.PreviousVersion).WithMany().HasForeignKey(d => d.PreviousVersionId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<InvoiceLine>().HasOne(l => l.ServiceOrder).WithMany().HasForeignKey(l => l.ServiceOrderId).OnDelete(DeleteBehavior.SetNull);

        // Audit log is immutable - no updates via app
        b.Entity<AuditLogEntry>(e => e.Property(a => a.Timestamp).HasDefaultValueSql("NOW()"));

        // Seed feature flags (also in SeedData)
    }
}
