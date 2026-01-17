namespace BoligInfo.Core.Models;

public class Address
{
    public long Id  { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Zipcode { get; set; }
    public string Street { get; set; }
    public string? Number { get; set; }
    public string? Suite {get; set;}
    public int? Floor { get; set; }
    public long HouseId { get; set; }
}