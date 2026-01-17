using BoligInfo.Core.Enums;

namespace BoligInfo.Core.Models;

public class CashFlow
{
    public long Id { get; init; }
    public CashFlowType Type { get; set; }
    public Frequency Frequency { get; set; }
    public double Amount { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long HouseId { get; set; }
}