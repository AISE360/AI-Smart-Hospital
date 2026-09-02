using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHospital.Domain.Entities;
using SmartHospital.Domain.Enums;
using SmartHospital.Infrastructure.Persistence;

namespace SmartHospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public InvoicesController(ApplicationDbContext db){_db=db;}

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _db.Invoices.Include(i=>i.Lines).Include(i=>i.Patient).OrderByDescending(i=>i.InvoiceDate).Take(100).Select(i=> new { i.Id, i.InvoiceNumber, Patient=i.Patient.FullName, i.TotalAmount, Status=i.Status.ToString(), i.InvoiceDate, Lines=i.Lines.Count }).ToListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _db.Invoices.Include(i=>i.Lines).Include(i=>i.Patient).FirstOrDefaultAsync(i=>i.Id==id));

    [HttpPost]
    [Authorize(Roles="Billing,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInvReq req)
    {
        var enc = await _db.Encounters.FindAsync(req.EncounterId);
        if(enc==null) return NotFound("Encounter not found");
        var lines = req.Lines.Select(l=> new InvoiceLine{ ServiceCode=l.ServiceCode, Description=l.Description, Category=l.Category, UnitPrice=l.UnitPrice, Quantity=l.Quantity, ServiceOrderId=l.ServiceOrderId }).ToList();
        var subtotal = lines.Sum(l=>l.LineTotal);
        var inv = new Invoice{ InvoiceNumber=$"INV-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000,9999)}", PatientId=req.PatientId, EncounterId=req.EncounterId, Status=InvoiceStatus.Finalized, SubTotal=subtotal, TotalAmount=subtotal - req.Discount + req.Tax, Discount=req.Discount, Tax=req.Tax, InvoiceDate=DateTime.UtcNow, Lines=lines };
        // mark service orders billed
        foreach(var line in lines.Where(l=>l.ServiceOrderId.HasValue))
        {
            var so = await _db.ServiceOrders.FindAsync(line.ServiceOrderId!.Value);
            if(so!=null) so.IsBilled=true;
        }
        _db.Invoices.Add(inv);
        await _db.SaveChangesAsync();
        return Ok(inv);
    }

    public record CreateInvReq(Guid PatientId, Guid? EncounterId, decimal Discount, decimal Tax, List<LineReq> Lines);
    public record LineReq(string ServiceCode, string Description, string Category, decimal UnitPrice, int Quantity, Guid? ServiceOrderId);
}
