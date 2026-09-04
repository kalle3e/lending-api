namespace LendingApi.Models;

/// <summary>
/// A flag that can be attached to an item (e.g. Fragile, AccessRestricted)
/// or represent a requester category as a "virtual" indicator.
/// </summary>
public class Indicator
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
