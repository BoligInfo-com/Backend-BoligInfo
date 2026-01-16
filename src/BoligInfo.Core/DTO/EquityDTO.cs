using BoligInfo.Core.Models;

namespace BoligInfo.Core.DTO;

public class EquityDto
{
    public long Id { get; init; }
    public string? Currency { get; set; }
    public Cash? Cash { get; set; }
    public ICollection<LoanDto>? Loans { get; set; }
}

public class CreateEquityDto
{
    public string? Currency { get; set; }
}

public class UpdateEquityDto
{
    public string? Currency { get; set; }
}