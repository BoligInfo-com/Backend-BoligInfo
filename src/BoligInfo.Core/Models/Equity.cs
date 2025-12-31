namespace BoligInfo.Core.Models;

public class Equity
{
    public long Id { get; set; }
    public string Currency { get; set; }
    
    // Navigation property for related loans
    public ICollection<Loan> Loans { get; set; }
}