using LendingApi.Data;
using LendingApi.Domain;
using LendingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LendingApi.Controllers;

public record CreateLoanDto(int ItemId, RequesterCategory RequesterCategory, string RequesterName);

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LendingContext _db;
    private readonly LoanValidationService _validator;

    public LoansController(LendingContext db, LoanValidationService validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>Submit a loan request. Blocked requests return 422 with reasons.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateLoanDto dto)
    {
        var item = await _db.Items.FindAsync(dto.ItemId);
        if (item is null) return NotFound(new { message = $"Item {dto.ItemId} not found" });

        var result = await _validator.ValidateAsync(dto.ItemId, dto.RequesterCategory);
        if (!result.IsApproved)
            return UnprocessableEntity(new { status = "Blocked", reasons = result.BlockReasons });

        var loan = new LoanRequest
        {
            ItemId = dto.ItemId,
            RequesterCategory = dto.RequesterCategory,
            RequesterName = dto.RequesterName,
            RequestedAtUtc = DateTime.UtcNow,
            Status = "Approved"
        };
        _db.LoanRequests.Add(loan);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = loan.Id },
            new { loan.Id, loan.ItemId, loan.Status, loan.RequestedAtUtc });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _db.LoanRequests.FindAsync(id);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.LoanRequests.OrderByDescending(l => l.RequestedAtUtc).ToListAsync());
}
