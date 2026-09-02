using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartHospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserRole = table.Column<string>(type: "text", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    EnabledForRolesJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureFlags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KpiSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    MetricName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    PreviousValue = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: true),
                    DimensionsJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mrn = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    AadhaarHash = table.Column<string>(type: "text", nullable: true),
                    AbhaId = table.Column<string>(type: "text", nullable: true),
                    Icd10Code = table.Column<string>(type: "text", nullable: true),
                    SnomedCode = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PharmacyItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GenericName = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Mrp = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    ReorderLevel = table.Column<int>(type: "integer", nullable: false),
                    ReorderQuantity = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    HsnCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SnomedCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MfaEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MfaSecret = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConsentRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GrantedById = table.Column<string>(type: "text", nullable: true),
                    HipId = table.Column<string>(type: "text", nullable: true),
                    ConsentArtifactId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpiryBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PharmacyItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ManufacturedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiryBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpiryBatches_PharmacyItems_PharmacyItemId",
                        column: x => x.PharmacyItemId,
                        principalTable: "PharmacyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PharmacyItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    QuantityOnHand = table.Column<int>(type: "integer", nullable: false),
                    QuantityReserved = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvgDailyConsumption = table.Column<double>(type: "double precision", nullable: true),
                    PredictedStockOutDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLevels_PharmacyItems_PharmacyItemId",
                        column: x => x.PharmacyItemId,
                        principalTable: "PharmacyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiOutputLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromptTemplate = table.Column<string>(type: "text", nullable: false),
                    PromptVersion = table.Column<string>(type: "text", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: false),
                    ModelVersion = table.Column<string>(type: "text", nullable: false),
                    InputSummary = table.Column<string>(type: "text", nullable: false),
                    OutputContent = table.Column<string>(type: "text", nullable: false),
                    TaskType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedById = table.Column<string>(type: "text", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    WasEdited = table.Column<bool>(type: "boolean", nullable: false),
                    EditedDiff = table.Column<string>(type: "text", nullable: true),
                    ConfidenceScore = table.Column<double>(type: "double precision", nullable: true),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiOutputLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiOutputLogs_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TokenNumber = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CheckedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Encounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedDoctorId = table.Column<string>(type: "text", nullable: true),
                    ChiefComplaint = table.Column<string>(type: "text", nullable: true),
                    SnomedCode = table.Column<string>(type: "text", nullable: true),
                    Icd10Code = table.Column<string>(type: "text", nullable: true),
                    LoincCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encounters_AspNetUsers_AssignedDoctorId",
                        column: x => x.AssignedDoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Encounters_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Encounters_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BedNumber = table.Column<string>(type: "text", nullable: false),
                    WardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentPatientId = table.Column<string>(type: "text", nullable: true),
                    OccupiedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpectedDischarge = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beds_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    History = table.Column<string>(type: "text", nullable: true),
                    Examination = table.Column<string>(type: "text", nullable: true),
                    Assessment = table.Column<string>(type: "text", nullable: true),
                    InvestigationOrders = table.Column<string>(type: "text", nullable: true),
                    PrescriptionDraft = table.Column<string>(type: "text", nullable: true),
                    FollowUp = table.Column<string>(type: "text", nullable: true),
                    RawTranscript = table.Column<string>(type: "text", nullable: true),
                    IsAiGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    AiOutputLogId = table.Column<Guid>(type: "uuid", nullable: true),
                    SignedById = table.Column<string>(type: "text", nullable: true),
                    SignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SignatureHash = table.Column<string>(type: "text", nullable: true),
                    PreviousVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_AiOutputLogs_AiOutputLogId",
                        column: x => x.AiOutputLogId,
                        principalTable: "AiOutputLogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_AspNetUsers_SignedById",
                        column: x => x.SignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_ClinicalNotes_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalTable: "ClinicalNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicalNotes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestCode = table.Column<string>(type: "text", nullable: false),
                    TestName = table.Column<string>(type: "text", nullable: false),
                    LoincCode = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderedById = table.Column<string>(type: "text", nullable: true),
                    CollectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrders_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    BedId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDischargeAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DischargedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdmittingDoctorId = table.Column<string>(type: "text", nullable: true),
                    Diagnosis = table.Column<string>(type: "text", nullable: true),
                    Icd10Code = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admissions_AspNetUsers_AdmittingDoctorId",
                        column: x => x.AdmittingDoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Admissions_Beds_BedId",
                        column: x => x.BedId,
                        principalTable: "Beds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admissions_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admissions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultValue = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    ReferenceRange = table.Column<string>(type: "text", nullable: true),
                    Criticality = table.Column<int>(type: "integer", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportedById = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    IsRouted = table.Column<bool>(type: "boolean", nullable: false),
                    RoutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResults_LabOrders_LabOrderId",
                        column: x => x.LabOrderId,
                        principalTable: "LabOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DischargeSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdmissionCourse = table.Column<string>(type: "text", nullable: true),
                    Investigations = table.Column<string>(type: "text", nullable: true),
                    Procedures = table.Column<string>(type: "text", nullable: true),
                    TreatmentGiven = table.Column<string>(type: "text", nullable: true),
                    ConditionAtDischarge = table.Column<string>(type: "text", nullable: true),
                    DischargeAdvice = table.Column<string>(type: "text", nullable: true),
                    FollowUpPlan = table.Column<string>(type: "text", nullable: true),
                    FullContent = table.Column<string>(type: "text", nullable: true),
                    IsAiGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    AiOutputLogId = table.Column<Guid>(type: "uuid", nullable: true),
                    SignedById = table.Column<string>(type: "text", nullable: true),
                    SignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PreviousVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DischargeSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DischargeSummaries_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DischargeSummaries_AiOutputLogs_AiOutputLogId",
                        column: x => x.AiOutputLogId,
                        principalTable: "AiOutputLogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DischargeSummaries_AspNetUsers_SignedById",
                        column: x => x.SignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DischargeSummaries_DischargeSummaries_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalTable: "DischargeSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DischargeSummaries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceCode = table.Column<string>(type: "text", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OrderedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderedById = table.Column<string>(type: "text", nullable: true),
                    IsBilled = table.Column<bool>(type: "boolean", nullable: false),
                    InvoiceLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    SnomedCode = table.Column<string>(type: "text", nullable: true),
                    LoincCode = table.Column<string>(type: "text", nullable: true),
                    CptCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimNumber = table.Column<string>(type: "text", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayerName = table.Column<string>(type: "text", nullable: false),
                    TpaCode = table.Column<string>(type: "text", nullable: true),
                    ClaimedAmount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    Icd10Code = table.Column<string>(type: "text", nullable: true),
                    ProcedureCode = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Claims_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceCode = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ServiceOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_ServiceOrders_ServiceOrderId",
                        column: x => x.ServiceOrderId,
                        principalTable: "ServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ClaimFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AssignedToId = table.Column<string>(type: "text", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimFlags_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClaimFlags_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DenialRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayerName = table.Column<string>(type: "text", nullable: false),
                    DenialReason = table.Column<string>(type: "text", nullable: false),
                    DenialCode = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    DeniedAmount = table.Column<decimal>(type: "numeric(18)", precision: 18, nullable: false),
                    DeniedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CorrectiveAction = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenialRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenialRecords_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_AdmittingDoctorId",
                table: "Admissions",
                column: "AdmittingDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_EncounterId",
                table: "Admissions",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_PatientId",
                table: "Admissions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AiOutputLogs_ApprovedById",
                table: "AiOutputLogs",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DepartmentId",
                table: "Appointments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beds_BedNumber",
                table: "Beds",
                column: "BedNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beds_WardId",
                table: "Beds",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimFlags_AssignedToId",
                table: "ClaimFlags",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimFlags_ClaimId",
                table: "ClaimFlags",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimNumber",
                table: "Claims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_InvoiceId",
                table: "Claims",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PatientId",
                table: "Claims",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_AiOutputLogId",
                table: "ClinicalNotes",
                column: "AiOutputLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_DoctorId",
                table: "ClinicalNotes",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_EncounterId",
                table: "ClinicalNotes",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_PatientId",
                table: "ClinicalNotes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_PreviousVersionId",
                table: "ClinicalNotes",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_SignedById",
                table: "ClinicalNotes",
                column: "SignedById");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_PatientId",
                table: "ConsentRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DenialRecords_ClaimId",
                table: "DenialRecords",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Code",
                table: "Departments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DischargeSummaries_AdmissionId",
                table: "DischargeSummaries",
                column: "AdmissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DischargeSummaries_AiOutputLogId",
                table: "DischargeSummaries",
                column: "AiOutputLogId");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeSummaries_PatientId",
                table: "DischargeSummaries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeSummaries_PreviousVersionId",
                table: "DischargeSummaries",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeSummaries_SignedById",
                table: "DischargeSummaries",
                column: "SignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_AssignedDoctorId",
                table: "Encounters",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_DepartmentId",
                table: "Encounters",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_PatientId",
                table: "Encounters",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryBatches_PharmacyItemId",
                table: "ExpiryBatches",
                column: "PharmacyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_Key",
                table: "FeatureFlags",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_ServiceOrderId",
                table: "InvoiceLines",
                column: "ServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AdmissionId",
                table: "Invoices",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EncounterId",
                table: "Invoices",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_EncounterId",
                table: "LabOrders",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_PatientId",
                table: "LabOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabOrderId",
                table: "LabResults",
                column: "LabOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Mrn",
                table: "Patients",
                column: "Mrn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyItems_Code",
                table: "PharmacyItems",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_AdmissionId",
                table: "ServiceOrders",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_EncounterId",
                table: "ServiceOrders",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_PharmacyItemId",
                table: "StockLevels",
                column: "PharmacyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_Code",
                table: "Wards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DepartmentId",
                table: "Wards",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ClaimFlags");

            migrationBuilder.DropTable(
                name: "ClinicalNotes");

            migrationBuilder.DropTable(
                name: "ConsentRecords");

            migrationBuilder.DropTable(
                name: "DenialRecords");

            migrationBuilder.DropTable(
                name: "DischargeSummaries");

            migrationBuilder.DropTable(
                name: "ExpiryBatches");

            migrationBuilder.DropTable(
                name: "FeatureFlags");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "KpiSnapshots");

            migrationBuilder.DropTable(
                name: "LabResults");

            migrationBuilder.DropTable(
                name: "StockLevels");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "AiOutputLogs");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "LabOrders");

            migrationBuilder.DropTable(
                name: "PharmacyItems");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "Beds");

            migrationBuilder.DropTable(
                name: "Encounters");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
