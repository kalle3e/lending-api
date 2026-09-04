using LendingApi.Data;
using LendingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LendingApi.Domain;

public record ValidationResult(bool IsApproved, IReadOnlyList<string> BlockReasons);

/// <summary>
/// Data-driven loan validation. The rules live in the IncompatibleIndicators
/// table: the item's indicators plus the requester's "virtual" indicator are
/// checked pairwise against the rule set. New rules are database inserts.
/// </summary>
public class LoanValidationService
{
    private readonly LendingContext _db;

    public LoanValidationService(LendingContext db) => _db = db;

    public async Task<ValidationResult> ValidateAsync(int itemId, RequesterCategory category)
    {
        var itemIndicatorIds = await _db.ItemIndicators
            .Where(ii => ii.ItemId == itemId)
            .Select(ii => ii.IndicatorId)
            .ToListAsync();

        var requesterIndicatorId = MapCategoryToIndicator(category);
        var requestIndicators = itemIndicatorIds
            .Concat(requesterIndicatorId is int rid ? new[] { rid } : Array.Empty<int>())
            .ToHashSet();

        var rules = await _db.IncompatibleIndicators.ToListAsync();

        var reasons = rules
            .Where(r => requestIndicators.Contains(r.IndicatorAId)
                     && requestIndicators.Contains(r.IndicatorBId))
            .Select(r => r.BlockMessage)
            .Distinct()
            .ToList();

        return new ValidationResult(reasons.Count == 0, reasons);
    }

    private static int? MapCategoryToIndicator(RequesterCategory category) => category switch
    {
        RequesterCategory.Public => 3,     // PUBLIC_REQ
        RequesterCategory.Researcher => 4, // RESEARCHER_REQ
        _ => null                          // Staff carry no virtual indicator
    };
}
