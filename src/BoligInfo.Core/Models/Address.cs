namespace BoligInfo.Core.Models;

public class Address
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