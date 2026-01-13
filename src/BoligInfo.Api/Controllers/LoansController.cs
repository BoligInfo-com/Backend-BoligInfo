using BoligInfo.Core.DTO;
using BoligInfo.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
    {
        var loans = await loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<LoanDto>> GetById(long id)
    {
        var loan = await loanService.GetLoanByIdAsync(id);
        if (loan == null)
            return NotFound();
        
        return Ok(loan);
    }

    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByEquityId(long equityId)
    {
        var loans = await loanService.GetLoansByEquityIdAsync(equityId);
        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create(CreateLoanDto createLoanDto)
    {
        var loan = await loanService.CreateLoanAsync(createLoanDto);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<LoanDto>> Update(long id, UpdateLoanDto updateLoanDto)
    {
        try
        {
            var loan = await loanService.UpdateLoanAsync(id, updateLoanDto);
            return Ok(loan);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        await loanService.DeleteLoanAsync(id);
        return NoContent();
    }
}