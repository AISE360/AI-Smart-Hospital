using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHospital.Api.Services;
using SmartHospital.Application.Dtos;
using SmartHospital.Domain.Entities;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<StaffUser> _users;
    private readonly SignInManager<StaffUser> _signIn;
    private readonly JwtTokenService _jwt;

    public AuthController(UserManager<StaffUser> users, SignInManager<StaffUser> signIn, JwtTokenService jwt)
    {
        _users = users; _signIn = signIn; _jwt = jwt;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest req)
    {
        var user = await _users.FindByNameAsync(req.Username);
        if (user == null || !user.IsActive) return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signIn.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials" });

        // MFA check for privileged roles
        if ((user.Role == Domain.Enums.StaffRole.Admin || user.Role == Domain.Enums.StaffRole.Billing) && user.MfaEnabled)
        {
            if (string.IsNullOrEmpty(req.MfaCode)) return BadRequest(new { message = "MFA code required", mfaRequired = true });
            // stub MFA: accept 123456
            if (req.MfaCode != "123456") return Unauthorized(new { message = "Invalid MFA code" });
        }

        var roles = await _users.GetRolesAsync(user);
        var token = _jwt.GenerateToken(user, roles);
        return new AuthResponse(token, user.UserName!, user.Role.ToString(), user.FullName, DateTime.UtcNow.AddHours(8));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = await _users.FindByIdAsync(userId!);
        if (user == null) return NotFound();
        var roles = await _users.GetRolesAsync(user);
        return Ok(new { user.Id, user.UserName, user.FullName, user.Email, Role = user.Role.ToString(), Roles = roles, user.DepartmentId });
    }

    [HttpPost("seed-demo-login")]
    [AllowAnonymous]
    public IActionResult SeedLogins()
    {
        return Ok(new
        {
            accounts = new[]
            {
                new { username="admin", password="Admin@123", role="Admin" },
                new { username="doctor1", password="Doctor@123", role="Doctor" },
                new { username="frontdesk", password="Front@123", role="FrontDesk" },
                new { username="billing", password="Bill@123", role="Billing" },
                new { username="pharmacy", password="Pharm@123", role="Pharmacy" },
                new { username="management", password="Manage@123", role="Management" },
                new { username="nurse1", password="Nurse@123", role="Nurse" },
            }
        });
    }
}
