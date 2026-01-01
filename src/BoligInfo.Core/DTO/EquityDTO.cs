namespace BoligInfo.Core.DTO;

public class EquityDto
{
    public long Id { get; init; }
    public string? Currency { get; set; }
    public List<LoanDto>? Loans { get; set; }
}

public class CreateEquityDto
{
    public string? Currency { get; set; }
}

public class UpdateEquityDto
{
    public string? Currency { get; set; }
}