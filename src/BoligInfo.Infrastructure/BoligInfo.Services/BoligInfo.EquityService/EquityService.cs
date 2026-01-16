using Boliginfo.CashRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.LoanRepository;

namespace BoligInfo.EquityService;

public class EquityService(
    IEquityRepository equityRepository,
    ILoanRepository loanRepository,
    ICashRepository cashRepository
    ) : IEquityService
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

        await equityRepository.UpdateAsync(equity);
        return MapToDto(equity);
    }

    public async Task DeleteEquityAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithLoansAsync(id);
        if (equity == null)
            throw new KeyNotFoundException($"Equity with ID {id} not found");

        // Manually delete related entities for in-memory database
        // (Real database would handle this via cascade delete configuration)
        if (equity.Loans != null && equity.Loans.Count != 0)
        {
            foreach (var loan in equity.Loans.ToList())
            {
                await loanRepository.DeleteAsync(loan.Id);
            }
        }
        
        // Delete cash if exists (need to get equity with cash)
        var equityWithCash = await equityRepository.GetByIdWithCashAsync(id);
        if (equityWithCash?.Cash != null)
        {
            await cashRepository.DeleteAsync(equityWithCash.Cash.Id);
        }

        // The cascade delete is configured in the database,
        // it should automatically delete loans and cash
        // In the production database
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