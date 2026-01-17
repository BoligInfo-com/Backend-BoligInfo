using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

public class AddressDto
{
    public long Id  { get; set; }
    public string Country { get; set; } = "Denmark";
    public string City { get; set; } = string.Empty;
    public string Zipcode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Number { get; set; }
    public string? Suite {get; set;}
    public int? Floor { get; set; }
    public long HouseId { get; set; }
}

public class CreateAddressDto
{
    [Required]
    [MaxLength(180, ErrorMessage = "Country cannot exceed 180 characters")]
    public string Country { get; set; } = "Denmark";
    
    [Required]
    [MaxLength(340, ErrorMessage = "Country cannot exceed 340 characters")]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(80, ErrorMessage = "Zipcode cannot exceed 80 characters")]
    public string Zipcode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(340, ErrorMessage = "Street cannot exceed 340 characters")]
    public string Street { get; set; } = string.Empty;
    
    [MaxLength(20, ErrorMessage = "Number cannot exceed 20 characters")]
    public string? Number { get; set; }
    
    [MaxLength(20, ErrorMessage = "Suite cannot exceed 20 characters")]
    public string? Suite {get; set;}
    
    public int? Floor { get; set; }
    
    [Required]
    public long HouseId { get; set; }
}

public class UpdateAddressDto
{
    [MaxLength(180, ErrorMessage = "Country cannot exceed 180 characters")]
    public string? Country { get; set; }
    
    [MaxLength(340, ErrorMessage = "Country cannot exceed 340 characters")]
    public string? City { get; set; } 
    
    [MaxLength(80, ErrorMessage = "Zipcode cannot exceed 80 characters")]
    public string? Zipcode { get; set; } 
    
    [MaxLength(340, ErrorMessage = "Street cannot exceed 340 characters")]
    public string? Street { get; set; }
    
    [MaxLength(20, ErrorMessage = "Number cannot exceed 20 characters")]
    public string? Number { get; set; }
    
    [MaxLength(20, ErrorMessage = "Suite cannot exceed 20 characters")]
    public string? Suite {get; set;}
    public int? Floor { get; set; }
}