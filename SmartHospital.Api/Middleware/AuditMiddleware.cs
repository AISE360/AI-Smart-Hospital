using SmartHospital.Application.Interfaces;

namespace SmartHospital.Api.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    public AuditMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IAuditService audit)
    {
        var start = DateTime.UtcNow;
        await _next(context);

        // Audit sensitive reads/writes (patient, financial, clinical)
        var path = context.Request.Path.Value ?? "";
        var isSensitive = path.Contains("patients", StringComparison.OrdinalIgnoreCase)
                       || path.Contains("clinical-notes", StringComparison.OrdinalIgnoreCase)
                       || path.Contains("invoices", StringComparison.OrdinalIgnoreCase)
                       || path.Contains("claims", StringComparison.OrdinalIgnoreCase);

        if (isSensitive && context.User.Identity?.IsAuthenticated == true)
        {
            var method = context.Request.Method;
            var status = context.Response.StatusCode;
            // Fire-and-forget audit (don't block response)
            _ = audit.LogAsync($"{method} {path} -> {status}", "HttpRequest", path, $"Method={method} Status={status}", true);
        }
    }
}
