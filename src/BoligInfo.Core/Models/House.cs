using BoligInfo.Core.Enums;

namespace BoligInfo.Core.Models;

public class House
{
    public long Id { get; init; }
    public double Price { get; set; }
    public int? NumberOfRooms { get; set; }
    public int? SquareMeters { get; set; }
    public EnergyLabel? EnergyLabel { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public long EquityId { get; set; }
    
    public ICollection<CashFlow>? CashFlows { get; set; }
}