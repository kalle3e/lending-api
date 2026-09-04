namespace LendingApi.Models;

public enum RequesterCategory
{
    Staff = 1,
    Public = 2,
    Researcher = 3
}

public class LoanRequest
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public RequesterCategory RequesterCategory { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; }
    public string Status { get; set; } = "Pending";
}
