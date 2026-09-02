using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles="Admin")]
public class AuditController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public AuditController(ApplicationDbContext db){_db=db;}

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? entityType, [FromQuery] int take=50)
    {
        var q = _db.AuditLogs.AsQueryable();
        if(!string.IsNullOrEmpty(entityType)) q=q.Where(a=>a.EntityType==entityType);
        return Ok(await q.OrderByDescending(a=>a.Timestamp).Take(take).ToListAsync());
    }

    [HttpGet("ai-outputs")]
    public async Task<IActionResult> AiOutputs() => Ok(await _db.AiOutputLogs.OrderByDescending(a=>a.CreatedAt).Take(100).ToListAsync());
}
