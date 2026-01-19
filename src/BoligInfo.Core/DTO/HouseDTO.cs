using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

public class HouseDto
{
    public long Id { get; init; }
    public double Price { get; set; }
    public int? NumberOfRooms { get; set; }
    public int? SquareMeters { get; set; }
    public string? EnergyLabel { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public long EquityId { get; set; }
    public AddressDto? Address { get; set; }
    public ICollection<CashFlowDto>? CashFlows { get; set; }
}

public class CreateHouseDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive")]
    public double Price { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Number of rooms must be zero or positive")]
    public int? NumberOfRooms { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Square meters must be zero or positive")]
    public int? SquareMeters { get; set; }
    
    public string? EnergyLabel { get; set; }
    
    public DateOnly? PurchaseDate { get; set; }
    
    [Required]
    public long EquityId { get; set; }
}

public class UpdateHouseDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive")]
    public double? Price { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Number of rooms must be zero or positive")]
    public int? NumberOfRooms { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Square meters must be zero or positive")]
    public int? SquareMeters { get; set; }
    
    public string? EnergyLabel { get; set; }
    
    public DateOnly? PurchaseDate { get; set; }
}