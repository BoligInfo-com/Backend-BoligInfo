namespace BoligInfo.Core.Models;

public class Equity
{
    public long Id { get; init; }
    public string? Currency { get; set; }
    
    public Cash? Cash { get; set; }
    public ICollection<Loan>? Loans { get; set; }
}