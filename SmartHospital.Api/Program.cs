using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartHospital.Api.Hubs;
using SmartHospital.Api.Middleware;
using SmartHospital.Api.Services;
using SmartHospital.Application.Interfaces;
using SmartHospital.Application.Services;
using SmartHospital.Domain.Entities;
using SmartHospital.Infrastructure.Ai;
using SmartHospital.Infrastructure.Persistence;
using SmartHospital.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// DB
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("SmartHospital"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connStr));
}

// Identity
builder.Services.AddIdentity<StaffUser, ApplicationRole>(opts =>
{
    opts.Password.RequireDigit = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequiredLength = 6;
    opts.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "DEV_ONLY_32_CHAR_SECRET_KEY_CHANGE_IN_PROD_12345";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SmartHospital";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SmartHospital.Client";
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(2)
    };
    // SignalR JWT from query string
    opts.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var accessToken = ctx.Request.Query["access_token"];
            var path = ctx.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                ctx.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

// Controllers + Swagger + SignalR + CORS + HttpContext
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI Smart Hospital API", Version = "v1", Description = "AI layer on top of HMIS/EMR for 50-bed hospital — all clinical AI outputs are drafts requiring human sign-off." });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type=ReferenceType.SecurityScheme, Id="Bearer"}}, Array.Empty<string>()}
    });
});
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" })
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddMemoryCache();

// Application services
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<RevenueReconciliationService>();
builder.Services.AddScoped<ClinicalNoteApprovalService>();
builder.Services.AddSingleton<IPharmacyForecastService, MovingAverageForecastService>();
builder.Services.AddScoped<JwtTokenService>();

// AI client: use HttpAiClient if AI_API_KEY set, else Stub
var aiKey = Environment.GetEnvironmentVariable("AI_API_KEY") ?? builder.Configuration["AI_API_KEY"];
if (!string.IsNullOrWhiteSpace(aiKey))
{
    builder.Services.AddHttpClient<IAiClient, HttpAiClient>();
}
else
{
    builder.Services.AddSingleton<IAiClient, StubAiClient>();
}
builder.Services.AddSingleton<ISttProvider, StubSttProvider>();

// Rate limiting - disabled for MVP (enable via AddRateLimiter in production)

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<StaffUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    // Ensure DB created for InMemory; for Npgsql apply migrations
    if (useInMemory) db.Database.EnsureCreated();
    else
    {
        try { db.Database.Migrate(); } catch { db.Database.EnsureCreated(); }
    }
    await SeedData.SeedAsync(db, userMgr, roleMgr);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();
app.MapHub<DashboardHub>("/hubs/dashboard");
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

// Stub STT for MVP
public class StubSttProvider : ISttProvider
{
    public Task<TranscriptionResult> TranscribeAsync(Stream audioStream, string languageCode = "en-IN", CancellationToken ct = default)
        => Task.FromResult(new TranscriptionResult("Stub transcription: patient reports fever and cough for 3 days.", 0.88, languageCode, TimeSpan.FromSeconds(12)));
}
