using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeatureFlagsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public FeatureFlagsController(ApplicationDbContext db){_db=db;}

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _db.FeatureFlags.ToListAsync());

    [HttpPatch("{key}/toggle")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> Toggle(string key)
    {
        var flag = await _db.FeatureFlags.FirstOrDefaultAsync(f=>f.Key==key);
        if(flag==null) return NotFound();
        flag.IsEnabled=!flag.IsEnabled;
        await _db.SaveChangesAsync();
        return Ok(flag);
    }
}
