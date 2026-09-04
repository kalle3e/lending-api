namespace LendingApi.Models;

public class ItemIndicator
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public int IndicatorId { get; set; }
    public Indicator Indicator { get; set; } = null!;
}
