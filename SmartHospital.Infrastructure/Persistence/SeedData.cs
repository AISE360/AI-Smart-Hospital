using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;

namespace SmartHospital.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<StaffUser> users, RoleManager<ApplicationRole> roles)
    {
        // Roles
        foreach (var r in Enum.GetNames<StaffRole>())
        {
            if (!await roles.RoleExistsAsync(r))
                await roles.CreateAsync(new ApplicationRole { Name = r, NormalizedName = r.ToUpperInvariant(), Description = r });
        }

        // Feature flags
        if (!db.FeatureFlags.Any())
        {
            var flags = new[]
            {
                new FeatureFlag{ Key="module.reception", DisplayName="AI Receptionist & Appointments", IsEnabled=true },
                new FeatureFlag{ Key="module.scribe", DisplayName="AI Medical Scribe", IsEnabled=true },
                new FeatureFlag{ Key="module.discharge", DisplayName="AI Discharge Summary", IsEnabled=true },
                new FeatureFlag{ Key="module.revenue", DisplayName="Revenue AI", IsEnabled=true },
                new FeatureFlag{ Key="module.claims", DisplayName="Insurance Claim Pre-check", IsEnabled=true },
                new FeatureFlag{ Key="module.pharmacy", DisplayName="Pharmacy AI", IsEnabled=true },
                new FeatureFlag{ Key="module.lab", DisplayName="Lab & Operations", IsEnabled=true },
                new FeatureFlag{ Key="module.command", DisplayName="Command Center", IsEnabled=true },
            };
            db.FeatureFlags.AddRange(flags);
            await db.SaveChangesAsync();
        }

        // Departments
        if (!db.Departments.Any())
        {
            var depts = new[]
            {
                new Department{ Name="General Medicine", Code="GM", Description="General Medicine" },
                new Department{ Name="Cardiology", Code="CARD", Description="Cardiology" },
                new Department{ Name="Orthopaedics", Code="ORTHO" },
                new Department{ Name="Paediatrics", Code="PAED" },
                new Department{ Name="Gynaecology", Code="GYN" },
                new Department{ Name="ENT", Code="ENT" },
                new Department{ Name="Ophthalmology", Code="OPHTH" },
                new Department{ Name="Dermatology", Code="DERM" },
                new Department{ Name="Emergency", Code="EMRG" },
                new Department{ Name="Radiology", Code="RAD" },
            };
            db.Departments.AddRange(depts);
            await db.SaveChangesAsync();
        }

        // Users
        var deptGm = await db.Departments.FirstAsync(d => d.Code=="GM");
        async Task EnsureUser(string username,string fullName,StaffRole role,string pwd,string empId, Guid? deptId)
        {
            var u = await users.FindByNameAsync(username);
            if (u==null)
            {
                u = new StaffUser{ UserName=username, Email=$"{username}@smarthospital.local", FullName=fullName, Role=role, EmployeeId=empId, DepartmentId=deptId, EmailConfirmed=true, IsActive=true };
                var res = await users.CreateAsync(u, pwd);
                if(res.Succeeded) await users.AddToRoleAsync(u, role.ToString());
            }
        }
        await EnsureUser("admin", "Hospital Administrator", StaffRole.Admin, "Admin@123", "EMP001", null);
        await EnsureUser("doctor1", "Dr. A. Sharma", StaffRole.Doctor, "Doctor@123", "DOC001", deptGm.Id);
        await EnsureUser("doctor2", "Dr. P. Nair", StaffRole.Doctor, "Doctor@123", "DOC002", deptGm.Id);
        await EnsureUser("frontdesk", "Reception User", StaffRole.FrontDesk, "Front@123", "F001", null);
        await EnsureUser("billing", "Finance User", StaffRole.Billing, "Bill@123", "B001", null);
        await EnsureUser("pharmacy", "Pharmacy User", StaffRole.Pharmacy, "Pharm@123", "P001", null);
        await EnsureUser("nurse1", "Nurse Sunita", StaffRole.Nurse, "Nurse@123", "N001", deptGm.Id);
        await EnsureUser("management", "Executive Viewer", StaffRole.Management, "Manage@123", "M001", null);
        await EnsureUser("labtech", "Lab Technician", StaffRole.LabTechnician, "Lab@123", "L001", null);

        // Wards & Beds
        if (!db.Wards.Any())
        {
            var wards = new[]
            {
                new Ward{ Name="General Ward - Male", Code="GW-M", Capacity=20, Floor=1, DepartmentId=deptGm.Id },
                new Ward{ Name="General Ward - Female", Code="GW-F", Capacity=15, Floor=1, DepartmentId=deptGm.Id },
                new Ward{ Name="ICU", Code="ICU", Capacity=6, Floor=2 },
                new Ward{ Name="Private Rooms", Code="PVT", Capacity=9, Floor=2 },
            };
            db.Wards.AddRange(wards);
            await db.SaveChangesAsync();
            var beds = new List<Bed>();
            foreach(var w in wards)
            {
                for(int i=1;i<=w.Capacity;i++)
                    beds.Add(new Bed{ BedNumber=$"{w.Code}-{i:D2}", WardId=w.Id, Status=BedStatus.Available });
            }
            // simulate 75% occupancy
            var rnd = new Random(42);
            foreach(var bed in beds.OrderBy(_=>rnd.Next()).Take((int)(beds.Count*0.75)))
            {
                bed.Status = BedStatus.Occupied;
                bed.OccupiedSince = DateTime.UtcNow.AddDays(-rnd.Next(1,5));
                bed.ExpectedDischarge = DateTime.UtcNow.AddDays(rnd.Next(1,4));
            }
            db.Beds.AddRange(beds);
            await db.SaveChangesAsync();
        }

        // Patients (fictional)
        if (!db.Patients.Any())
        {
            var patients = new List<Patient>();
            var first = new[]{"Aarav","Vivaan","Aditya","Sai","Arjun","Riya","Diya","Ananya","Priya","Kavya","Rohan","Ishaan","Samar","Neha","Pooja"};
            var last = new[]{"Sharma","Verma","Gupta","Patel","Kumar","Singh","Reddy","Nair","Desai","Joshi","Mehta","Kapoor","Iyer","Chopra","Malhotra"};
            var rnd = new Random(123);
            for(int i=0;i<40;i++)
            {
                var fn = first[rnd.Next(first.Length)];
                var ln = last[rnd.Next(last.Length)];
                patients.Add(new Patient{
                    Mrn=$"MRN{(1001+i):D6}",
                    FullName=$"{fn} {ln}",
                    FirstName=fn, LastName=ln,
                    Gender=(Gender)rnd.Next(0,2),
                    DateOfBirth=DateTime.UtcNow.AddYears(-rnd.Next(18,75)).AddDays(-rnd.Next(0,365)),
                    Phone=$"9{rnd.Next(100000000,999999999)}",
                    Email=$"{fn.ToLower()}{i}@example.test",
                    Address=$"{rnd.Next(1,200)}, MG Road, Pune",
                    AbhaId=$"ABHA-{rnd.Next(100000,999999)}"
                });
            }
            db.Patients.AddRange(patients);
            await db.SaveChangesAsync();
        }

        // Appointments, Encounters, ServiceOrders, Invoices, Pharmacy, Labs, KPIs
        if (!db.Encounters.Any())
        {
            var patients = await db.Patients.Take(20).ToListAsync();
            var doctor = await users.FindByNameAsync("doctor1");
            var rnd = new Random(999);
            var encounters = new List<Encounter>();
            var serviceOrders = new List<ServiceOrder>();
            var appointments = new List<Appointment>();

            foreach(var p in patients)
            {
                var appt = new Appointment{
                    PatientId=p.Id, DoctorId=doctor!.Id, DepartmentId=deptGm.Id,
                    ScheduledAt=DateTime.UtcNow.AddDays(rnd.Next(-2,5)).AddHours(rnd.Next(9,17)),
                    Status= (AppointmentStatus)rnd.Next(0,3),
                    TokenNumber=$"T{rnd.Next(100,999)}", Reason="Fever / general consultation"
                };
                appointments.Add(appt);

                var enc = new Encounter{
                    PatientId=p.Id, Type=EncounterType.OPD, Status=EncounterStatus.Finished,
                    StartTime=DateTime.UtcNow.AddDays(-rnd.Next(0,10)), EndTime=DateTime.UtcNow,
                    DepartmentId=deptGm.Id, AssignedDoctorId=doctor.Id, ChiefComplaint="Fever, cough, headache"
                };
                encounters.Add(enc);
            }
            db.Appointments.AddRange(appointments);
            db.Encounters.AddRange(encounters);
            await db.SaveChangesAsync();

            // ServiceOrders per encounter
            var serviceCatalog = new[]{
                ("CONS001","Consultation - General", "Consultation", 600m),
                ("LAB001","CBC", "Lab", 400m),
                ("LAB002","RFT", "Lab", 800m),
                ("RAD001","Chest X-Ray", "Radiology", 1200m),
                ("PROC001","Nebulization", "Procedure", 500m),
                ("BED001","Bed Charge - General", "BedCharge", 2500m),
                ("CONS002","Consumables", "Consumable", 350m),
            };
            foreach(var enc in encounters)
            {
                var count = rnd.Next(2,5);
                for(int i=0;i<count;i++)
                {
                    var svc = serviceCatalog[rnd.Next(serviceCatalog.Length)];
                    serviceOrders.Add(new ServiceOrder{
                        EncounterId=enc.Id, ServiceCode=svc.Item1, ServiceName=svc.Item2, Category=svc.Item3, UnitPrice=svc.Item4, Quantity=1, OrderedAt=enc.StartTime.AddHours(1), IsBilled=false
                    });
                }
            }
            db.ServiceOrders.AddRange(serviceOrders);
            await db.SaveChangesAsync();

            // Invoices - intentionally leave ~30% unbilled to demo leakage
            var invoices = new List<Invoice>();
            var invoiceLines = new List<InvoiceLine>();
            foreach(var enc in encounters.Take(12))
            {
                var sos = serviceOrders.Where(s=>s.EncounterId==enc.Id).ToList();
                // only bill first 70% of services
                var toBill = sos.Take((int)Math.Ceiling(sos.Count*0.7)).ToList();
                if(!toBill.Any()) continue;
                var inv = new Invoice{
                    InvoiceNumber=$"INV-{DateTime.UtcNow:yyyyMMdd}-{rnd.Next(1000,9999)}",
                    PatientId=enc.PatientId, EncounterId=enc.Id, Status=InvoiceStatus.Finalized,
                    InvoiceDate=DateTime.UtcNow.AddDays(-rnd.Next(0,5)), TotalAmount=toBill.Sum(s=>s.TotalPrice)
                };
                invoices.Add(inv);
                foreach(var so in toBill)
                {
                    so.IsBilled = true;
                }
            }
            db.Invoices.AddRange(invoices);
            await db.SaveChangesAsync();
            // Create InvoiceLines
            foreach(var inv in invoices)
            {
                var sos = serviceOrders.Where(s=>s.EncounterId==inv.EncounterId && s.IsBilled).ToList();
                foreach(var so in sos)
                {
                    invoiceLines.Add(new InvoiceLine{
                        InvoiceId=inv.Id, ServiceCode=so.ServiceCode, Description=so.ServiceName, Category=so.Category, UnitPrice=so.UnitPrice, Quantity=so.Quantity, ServiceOrderId=so.Id
                    });
                }
                inv.SubTotal = invoiceLines.Where(l=>l.InvoiceId==inv.Id).Sum(l=>l.LineTotal);
                inv.TotalAmount = inv.SubTotal;
            }
            db.InvoiceLines.AddRange(invoiceLines);
            await db.SaveChangesAsync();

            // Claims
            var payers = new[]{"Star Health","ICICI Lombard","HDFC Ergo","Niva Bupa"};
            foreach(var inv in invoices.Take(6))
            {
                var claim = new Claim{
                    ClaimNumber=$"CLM-{rnd.Next(10000,99999)}",
                    InvoiceId=inv.Id, PatientId=inv.PatientId,
                    PayerName=payers[rnd.Next(payers.Length)],
                    ClaimedAmount=inv.TotalAmount,
                    Status= (ClaimStatus)rnd.Next(1,4),
                    SubmittedAt=DateTime.UtcNow.AddDays(-rnd.Next(1,10)),
                    Icd10Code="J06.9", ProcedureCode="CONS001"
                };
                db.Claims.Add(claim);
            }
            await db.SaveChangesAsync();
            // Denials for analytics
            var claims = await db.Claims.ToListAsync();
            foreach(var c in claims.Take(3))
            {
                db.DenialRecords.Add(new DenialRecord{ ClaimId=c.Id, PayerName=c.PayerName, DenialReason="Missing discharge summary", DenialCode="DOC001", Department="General Medicine", DeniedAmount=c.ClaimedAmount*0.2m, DeniedAt=DateTime.UtcNow.AddDays(-2) });
            }
            await db.SaveChangesAsync();
        }

        if (!db.PharmacyItems.Any())
        {
            var items = new[]{
                new PharmacyItem{ Code="MED001", Name="Paracetamol 500mg", GenericName="Paracetamol", Category="Analgesic", Manufacturer="Cipla", Unit="Strip", Mrp=30, CostPrice=18, ReorderLevel=100, ReorderQuantity=200 },
                new PharmacyItem{ Code="MED002", Name="Amoxicillin 500mg", GenericName="Amoxicillin", Category="Antibiotic", Manufacturer="Sun Pharma", Unit="Strip", Mrp=85, CostPrice=50, ReorderLevel=80, ReorderQuantity=150 },
                new PharmacyItem{ Code="MED003", Name="Atorvastatin 20mg", GenericName="Atorvastatin", Category="Cardiac", Manufacturer="Dr Reddy", Unit="Strip", Mrp=120, CostPrice=70, ReorderLevel=60, ReorderQuantity=120 },
                new PharmacyItem{ Code="MED004", Name="Omeprazole 20mg", GenericName="Omeprazole", Category="Gastro", Manufacturer="Zydus", Unit="Strip", Mrp=45, CostPrice=25, ReorderLevel=90, ReorderQuantity=180 },
                new PharmacyItem{ Code="MED005", Name="Salbutamol Inhaler", GenericName="Salbutamol", Category="Respiratory", Manufacturer="Cipla", Unit="Piece", Mrp=220, CostPrice=150, ReorderLevel=40, ReorderQuantity=80 },
            };
            db.PharmacyItems.AddRange(items);
            await db.SaveChangesAsync();
            var rnd = new Random(777);
            foreach(var item in items)
            {
                var qty = rnd.Next(20, 300);
                var avgDaily = rnd.Next(5, 25);
                db.StockLevels.Add(new StockLevel{
                    PharmacyItemId=item.Id, Location="Main Store", QuantityOnHand=qty, QuantityReserved=rnd.Next(0,10),
                    AvgDailyConsumption=avgDaily,
                    PredictedStockOutDate=DateTime.UtcNow.AddDays(qty/(double)avgDaily)
                });
                // batches
                for(int b=0;b<2;b++)
                {
                    var expiry = b==0 ? DateTime.UtcNow.AddDays(rnd.Next(10, 80)) : DateTime.UtcNow.AddDays(rnd.Next(120, 400));
                    db.ExpiryBatches.Add(new ExpiryBatch{
                        PharmacyItemId=item.Id, BatchNumber=$"B{rnd.Next(1000,9999)}", Quantity=rnd.Next(10,100),
                        ManufacturedDate=DateTime.UtcNow.AddDays(-rnd.Next(30,200)), ExpiryDate=expiry
                    });
                }
            }
            await db.SaveChangesAsync();
        }

        if (!db.LabOrders.Any())
        {
            var patients = await db.Patients.Take(10).ToListAsync();
            var encounters = await db.Encounters.Take(10).ToListAsync();
            var rnd = new Random(555);
            var tests = new[]{ ("CBC","Hemogram"), ("RFT","Renal Function"), ("LFT","Liver Function"), ("CRP","C-Reactive Protein"), ("ECG","ECG") };
            for(int i=0;i<patients.Count;i++)
            {
                var t = tests[rnd.Next(tests.Length)];
                var order = new LabOrder{
                    PatientId=patients[i].Id, EncounterId=encounters[i].Id, TestCode=t.Item1, TestName=t.Item2, LoincCode="0000-0",
                    Status= LabOrderStatus.Completed, OrderedAt=DateTime.UtcNow.AddDays(-rnd.Next(1,5)), Priority= rnd.NextDouble()>0.7 ? "Urgent" : "Routine",
                    CollectedAt=DateTime.UtcNow.AddDays(-1), CompletedAt=DateTime.UtcNow
                };
                db.LabOrders.Add(order);
                await db.SaveChangesAsync();
                var critical = rnd.NextDouble() > 0.85;
                db.LabResults.Add(new LabResult{
                    LabOrderId=order.Id, ResultValue= critical? "Critical high" : "Normal", Criticality= critical? LabResultCriticality.Critical : LabResultCriticality.Normal,
                    ReportedAt=DateTime.UtcNow, IsRouted= !critical
                });
            }
            await db.SaveChangesAsync();
        }

        if (!db.KpiSnapshots.Any())
        {
            var today = DateTime.UtcNow.Date;
            var rnd = new Random(321);
            var kpis = new[]
            {
                new KpiSnapshot{ Date=today, Category=KpiCategory.Bed, MetricName="Occupancy %", Value= rnd.Next(70,86), Unit="%" , PreviousValue=72 },
                new KpiSnapshot{ Date=today, Category=KpiCategory.Opd, MetricName="OPD Volume", Value= rnd.Next(120,160), Unit="count", PreviousValue=130 },
                new KpiSnapshot{ Date=today, Category=KpiCategory.Revenue, MetricName="Revenue / Day", Value= rnd.Next(500000,700000), Unit="INR", PreviousValue=550000 },
                new KpiSnapshot{ Date=today, Category=KpiCategory.Claims, MetricName="Rejection Rate", Value= rnd.Next(8,18), Unit="%", PreviousValue=14 },
                new KpiSnapshot{ Date=today, Category=KpiCategory.Pharmacy, MetricName="Stock-out Risk Items", Value= rnd.Next(2,6), Unit="count", PreviousValue=3 },
            };
            db.KpiSnapshots.AddRange(kpis);
            await db.SaveChangesAsync();
        }

        // Consent records
        if (!db.ConsentRecords.Any())
        {
            var patients = await db.Patients.Take(5).ToListAsync();
            foreach(var p in patients)
                db.ConsentRecords.Add(new ConsentRecord{ PatientId=p.Id, Purpose="Treatment", Status=ConsentStatus.Granted, GrantedAt=DateTime.UtcNow.AddDays(-10) });
            await db.SaveChangesAsync();
        }
    }
}
