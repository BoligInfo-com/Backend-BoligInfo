using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

/// <summary>
/// Data transfer object representing a house including
/// its address and related cash flows.
/// </summary>
public class HouseDto
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
    public string? EnergyLabel { get; set; }
    
    /// <summary>
    /// Date when the house was purchased.
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
    
    /// <summary>
    /// Identifier of the equity this house belongs to.
    /// </summary>
    public long EquityId { get; set; }
    
    /// <summary>
    /// Address associated with the house.
    /// </summary>
    public AddressDto? Address { get; set; }
    
    /// <summary>
    /// Collection of cash flows related to the house.
    /// </summary>
    public ICollection<CashFlowDto>? CashFlows { get; set; }
}

/// <summary>
/// DTO used when creating a new house.
/// </summary>
public class CreateHouseDto
{
    /// <summary>
    /// Purchase price of the house.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive")]
    public double Price { get; set; }
    
    /// <summary>
    /// Number of rooms in the house.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Number of rooms must be zero or positive")]
    public int? NumberOfRooms { get; set; }
    
    /// <summary>
    /// Total living area in square meters.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Square meters must be zero or positive")]
    public int? SquareMeters { get; set; }
    
    /// <summary>
    /// Energy efficiency label of the house.
    /// </summary>
    public string? EnergyLabel { get; set; }
    
    /// <summary>
    /// Date when the house was purchased.
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
    
    /// <summary>
    /// Identifier of the equity the house belongs to.
    /// </summary>
    [Required]
    public long EquityId { get; set; }
}

/// <summary>
/// DTO used when updating an existing house.
/// All fields are optional.
/// </summary>
public class UpdateHouseDto
{
    /// <summary>
    /// Updated purchase price of the house.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive")]
    public double? Price { get; set; }
    
    /// <summary>
    /// Updated number of rooms.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Number of rooms must be zero or positive")]
    public int? NumberOfRooms { get; set; }
    
    /// <summary>
    /// Updated living area in square meters.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Square meters must be zero or positive")]
    public int? SquareMeters { get; set; }
    
    /// <summary>
    /// Updated energy efficiency label.
    /// </summary>
    public string? EnergyLabel { get; set; }
    
    
    /// <summary>
    /// Updated purchase date.
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
}