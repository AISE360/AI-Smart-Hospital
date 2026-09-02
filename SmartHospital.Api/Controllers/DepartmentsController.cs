using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Entities;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public DepartmentsController(ApplicationDbContext db){_db=db;}

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _db.Departments.ToListAsync());

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> Create([FromBody] Department dept)
    {
        dept.Id = Guid.NewGuid();
        _db.Departments.Add(dept);
        await _db.SaveChangesAsync();
        return Ok(dept);
    }
}
