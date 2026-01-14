using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers;

public interface ILoansController
{
    Task<ActionResult<IEnumerable<LoanDto>>> GetAll();
    Task<ActionResult<LoanDto>> GetById(long id);
    Task<ActionResult<IEnumerable<LoanDto>>> GetByEquityId(long equityId);
    Task<ActionResult<LoanDto>> Create(CreateLoanDto createLoanDto);
    Task<ActionResult<LoanDto>> Update(long id, UpdateLoanDto updateLoanDto);
    Task<ActionResult> Delete(long id);
}