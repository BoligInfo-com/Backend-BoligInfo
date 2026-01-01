using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;

namespace BoligInfo.Services;

public class EquityService : IEquityService
{
    private readonly IEquityRepository _equityRepository;

    public EquityService(IEquityRepository equityRepository)
    {
        _equityRepository = equityRepository;
    }

    public async Task<IEnumerable<EquityDto>> GetAllEquitiesAsync()
    {
        var equities = await _equityRepository.GetAllAsync();
        return equities.Select(MapToDto);
    }

    public async Task<EquityDto?> GetEquityByIdAsync(long id)
    {
        var equity = await _equityRepository.GetByIdAsync(id);
        return equity == null ? null : MapToDto(equity);
    }

    public async Task<EquityDto?> GetEquityWithLoansAsync(long id)
    {
        var equity = await _equityRepository.GetByIdWithLoansAsync(id);
        return equity == null ? null : MapToDtoWithLoans(equity);
    }

    public async Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto)
    {
        var equity = new Equity
        {
            Currency = createEquityDto.Currency ?? "DKK"
        };

        var createdEquity = await _equityRepository.AddAsync(equity);
        return MapToDto(createdEquity);
    }

    public async Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto)
    {
        var equity = await _equityRepository.GetByIdAsync(id);
        if (equity == null)
            throw new KeyNotFoundException($"Equity with ID {id} not found");

        if (updateEquityDto.Currency != null)
            equity.Currency = updateEquityDto.Currency;

        await _equityRepository.UpdateAsync(equity);
        return MapToDto(equity);
    }

    public async Task DeleteEquityAsync(long id)
    {
        await _equityRepository.DeleteAsync(id);
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
}