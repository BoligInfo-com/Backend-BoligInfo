using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

public class CashFlowDto
{
    public long Id { get; init; }
    public string Type { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long HouseId { get; set; }
}

public class CreateCashFlowDto
{
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Frequency { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be zero or positive")]
    public double Amount { get; set; }
    
    [Required]
    [MaxLength(60, ErrorMessage = "Name cannot exceed 60 characters")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1400, ErrorMessage = "Description cannot exceed 1400 characters")]
    public string? Description { get; set; }
    
    [Required]
    public long HouseId { get; set; }
}

public class UpdateCashFlowDto
{
    public string? Type { get; set; }
    
    public string? Frequency { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be zero or positive")]
    public double? Amount { get; set; }
    
    [MaxLength(60, ErrorMessage = "Name cannot exceed 60 characters")]
    public string? Name { get; set; }
    
    [MaxLength(1400, ErrorMessage = "Description cannot exceed 1400 characters")]
    public string? Description { get; set; }
}