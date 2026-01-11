namespace BoligInfo.Core.DTO;

public class CashDTO
{
    public long Id { get; init; }
    public double CashAmount { get; set; }
    public long EquityId { get; set; }
}

public class CreateCashDto
{
    public double CashAmount { get; set; }
    public long EquityId { get; set; }
}

public class UpdateCashDto
{
    public double CashAmount { get; set; }
}