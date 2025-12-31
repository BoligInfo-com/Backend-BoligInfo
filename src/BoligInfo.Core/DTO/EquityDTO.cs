namespace BoligInfo.Core.DTOs;

public class EquityDto
{
    public long Id { get; set; }
    public string Currency { get; set; }
    public List<LoanDto>? Loans { get; set; }
}

public class CreateEquityDto
{
    public string Currency { get; set; }
}

public class UpdateEquityDto
{
    public string? Currency { get; set; }
}