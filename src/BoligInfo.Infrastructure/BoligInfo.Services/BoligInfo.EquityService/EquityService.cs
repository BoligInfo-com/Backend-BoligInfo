using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;

namespace BoligInfo.EquityService;

public class EquityService(IEquityRepository equityRepository) : IEquityService
{
    public async Task<IEnumerable<EquityDto>> GetAllEquitiesAsync()
    {
        var equities = await equityRepository.GetAllAsync();
        return equities.Select(MapToDto);
    }

    public async Task<EquityDto?> GetEquityByIdAsync(long id)
    {
        var equity = await equityRepository.GetByIdAsync(id);
        return equity == null ? null : MapToDto(equity);
    }

    public async Task<EquityDto?> GetEquityWithLoansAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithLoansAsync(id);
        return equity == null ? null : MapToDtoWithLoans(equity);
    }

    public async Task<EquityDto?> GetEquityWithCashAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithCashAsync(id);
        return equity == null ? null : MapToDtoWithCash(equity);
    }

    public async Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto)
    {
        var equity = new Equity
        {
            Currency = createEquityDto.Currency ?? "DKK"
        };

        var createdEquity = await equityRepository.AddAsync(equity);
        return MapToDto(createdEquity);
    }

    public async Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto)
    {
        var equity = await equityRepository.GetByIdAsync(id);
        if (equity == null)
            throw new KeyNotFoundException($"Equity with ID {id} not found");

        if (updateEquityDto.Currency != null)
            equity.Currency = updateEquityDto.Currency;

        await equityRepository.SaveChangesAsync();
        return MapToDto(equity);
    }

    public async Task DeleteEquityAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithCashAsync(id);
        if (equity == null)
            throw new KeyNotFoundException($"Equity with ID {id} not found");

        if (equity.Loans != null)
            equity.Loans.Clear(); 
        
        equity.Cash = null;
        
        await equityRepository.DeleteAsync(id);
    }

    private static EquityDto MapToDto(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency
        };
    }

    private static EquityDto MapToDtoWithLoans(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency,
            Loans = equity.Loans?.Select(l => new LoanDto
            {
                Id = l.Id,
                LoanType = l.LoanType.ToString(),
                LoanAmount = l.LoanAmount,
                InterestRate = l.InterestRate,
                LoanLifetime = l.LoanLifetime,
                EquityId = l.EquityId
            }).ToList()
        };
    }
    
    private static EquityDto MapToDtoWithCash(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency,
            Cash = equity.Cash != null ? new Cash
            {
                Id = equity.Cash.Id,
                CashAmount = equity.Cash.CashAmount,
                EquityId = equity.Cash.EquityId
            } : null
        };
    }
}