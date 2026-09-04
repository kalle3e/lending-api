namespace LendingApi.Models;

/// <summary>
/// Data-driven blocking rule: when indicator A and indicator B appear together
/// on a request (item indicators + requester's virtual indicator), the loan is
/// blocked with the given message. Adding a rule is a database insert -
/// no code change, no redeploy.
/// </summary>
public class IncompatibleIndicator
{
    public int Id { get; set; }
    public int IndicatorAId { get; set; }
    public int IndicatorBId { get; set; }
    public string BlockMessage { get; set; } = string.Empty;
}
