namespace LendingApi.Models;

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public List<ItemIndicator> Indicators { get; set; } = new();
}
