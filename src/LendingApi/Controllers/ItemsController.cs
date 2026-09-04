using LendingApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LendingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly LendingContext _db;
    public ItemsController(LendingContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Items
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Barcode,
                Indicators = i.Indicators.Select(ii => ii.Indicator.Code)
            })
            .ToListAsync());
}
