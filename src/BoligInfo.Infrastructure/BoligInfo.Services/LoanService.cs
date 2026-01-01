using BoligInfo.Core.DTOs;
using BoligInfo.Core.Models;
using BoligInfo.Core.Enums;
using BoligInfo.LoanRepository;

namespace BoligInfo.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;

    public LoanService(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
    {
        var loans = await _loanRepository.GetAllAsync();
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(long id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByEquityIdAsync(long equityId)
    {
        var loans = await _loanRepository.GetByEquityIdAsync(equityId);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        var loan = new Loan
        {
            LoanType = string.IsNullOrEmpty(createLoanDto.LoanType) 
                ? null 
                : Enum.Parse<LoanType>(createLoanDto.LoanType),
            LoanAmount = createLoanDto.LoanAmount,
            InterestRate = createLoanDto.InterestRate,
            LoanLifetime = createLoanDto.LoanLifetime,
            EquityId = createLoanDto.EquityId
        };

        var createdLoan = await _loanRepository.AddAsync(loan);
        return MapToDto(createdLoan);
    }

    public async Task<LoanDto> UpdateLoanAsync(long id, UpdateLoanDto updateLoanDto)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        if (loan == null)
            throw new KeyNotFoundException($"Loan with ID {id} not found");

        if (updateLoanDto.LoanType != null)
            loan.LoanType = Enum.Parse<LoanType>(updateLoanDto.LoanType);
        
        if (updateLoanDto.LoanAmount.HasValue)
            loan.LoanAmount = updateLoanDto.LoanAmount.Value;
        
        if (updateLoanDto.InterestRate.HasValue)
            loan.InterestRate = updateLoanDto.InterestRate.Value;
        
        if (updateLoanDto.LoanLifetime.HasValue)
            loan.LoanLifetime = updateLoanDto.LoanLifetime.Value;

        await _loanRepository.UpdateAsync(loan);
        return MapToDto(loan);
    }

    public async Task DeleteLoanAsync(long id)
    {
        await _loanRepository.DeleteAsync(id);
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            LoanType = loan.LoanType?.ToString() ?? string.Empty,
            LoanAmount = loan.LoanAmount,
            InterestRate = loan.InterestRate,
            LoanLifetime = loan.LoanLifetime,
            EquityId = loan.EquityId
        };
    }
}