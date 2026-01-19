using BoligInfo.Core.Enums;

namespace BoligInfo.Core.Models;

/// <summary>
/// Represents a residential property owned under an equity.
/// A house can have an address and multiple cash flows (income/expenses).
/// </summary>
public class House
{
    /// <summary>
    /// Unique identifier for the house.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Purchase price or valuation of the house.
    /// </summary>
    public double Price { get; set; }
    
    /// <summary>
    /// Number of rooms in the house.
    /// </summary>
    public int? NumberOfRooms { get; set; }
    
    /// <summary>
    /// Total living area in square meters.
    /// </summary>
    public int? SquareMeters { get; set; }
    
    /// <summary>
    /// Energy efficiency label of the house.
    /// </summary>
    public EnergyLabel? EnergyLabel { get; set; }
    
    /// <summary>
    /// Date when the house was purchased.
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
    
    /// <summary>
    /// Date when the house was purchased.
    /// </summary>
    public long EquityId { get; set; }
    
    /// <summary>
    /// Address associated with the house.
    /// </summary>
    public Address? Address { get; set; }
    
    /// <summary>
    /// Collection of cash flows related to the house
    /// (e.g. rent income, maintenance expenses).
    /// </summary>
    public ICollection<CashFlow>? CashFlows { get; set; }
}