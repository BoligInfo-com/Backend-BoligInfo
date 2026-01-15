using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.LoanRepository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BoligInfo.LoanService;

public class LoanService(ILoanRepository loanRepository, IEquityRepository equityRepository) : ILoanService
{
    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
    {
        var loans = await loanRepository.GetAllAsync();
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(long id)
    {
        var loan = await loanRepository.GetByIdAsync(id);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByEquityIdAsync(long equityId)
    {
        var loans = await loanRepository.GetByEquityIdAsync(equityId);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        var equity = await equityRepository.GetByIdAsync(createLoanDto.EquityId);
        if (equity == null) 
            throw new KeyNotFoundException($"Equity {createLoanDto.EquityId} not found");
        
        var loan = new Loan
        {
            LoanType = string.IsNullOrEmpty(createLoanDto.LoanType) 
                ? null 
                : Enum.Parse<LoanType>(createLoanDto.LoanType),
            LoanAmount = createLoanDto.LoanAmount,
            InterestRate = createLoanDto.InterestRate,
            LoanLifetime = createLoanDto.LoanLifetime,
            EquityId = createLoanDto.EquityId,
        };
        
        equity.Loans ??= new List<Loan>();
        equity.Loans.Add(loan);

        await loanRepository.AddAsync(loan);
        
        return MapToDto(loan);
    }

    public async Task<LoanDto> UpdateLoanAsync(long id, UpdateLoanDto updateLoanDto)
    {
        var loan = await loanRepository.GetByIdAsync(id);
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

        await loanRepository.UpdateAsync(loan);
        return MapToDto(loan);
    }

    public async Task DeleteLoanAsync(long id)
    {
        var exists = await loanRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Loan {id} not found");;

        await loanRepository.DeleteAsync(id);
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