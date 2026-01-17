namespace BoligInfo.Core.Models;

public class Cash
{
    public long Id { get; init; }
    public double CashAmount { get; set; }
    public long EquityId { get; set; }
}