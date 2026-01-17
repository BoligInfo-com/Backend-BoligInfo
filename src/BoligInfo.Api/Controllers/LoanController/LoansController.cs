using BoligInfo.Core.DTO;
using BoligInfo.LoanService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.LoanController;

/// <summary>
/// Controller for managing loans.
/// Provides endpoints to retrieve, create, update, and delete <see cref="LoanDto"/> entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoansController(ILoanService loanService) : ControllerBase, ILoansController
{
    
    /// <summary>
    /// Retrieves all loans.
    /// </summary>
    /// <returns>A list of <see cref="LoanDto"/> objects.</returns>
    /// <response code="200">Returns all loans.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
    {
        var loans = await loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    /// <summary>
    /// Retrieves a loan by ID.
    /// </summary>
    /// <param name="id">The ID of the loan.</param>
    /// <returns>The <see cref="LoanDto"/> with the specified ID.</returns>
    /// <response code="200">Returns the loan.</response>
    /// <response code="404">Loan not found.</response>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<LoanDto>> GetById(long id)
    {
        var loan = await loanService.GetLoanByIdAsync(id);
        if (loan == null)
            return NotFound();
        
        return Ok(loan);
    }

    /// <summary>
    /// Retrieves all loans associated with a specific equity.
    /// </summary>
    /// <param name="equityId">The ID of the equity.</param>
    /// <returns>List of loans for the given equity.</returns>
    /// <response code="200">Returns loans for the equity.</response>
    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByEquityId(long equityId)
    {
        var loans = await loanService.GetLoansByEquityIdAsync(equityId);
        return Ok(loans);
    }

    /// <summary>
    /// Creates a new loan.
    /// </summary>
    /// <param name="createLoanDto">Data for the new loan.</param>
    /// <returns>The created <see cref="LoanDto"/>.</returns>
    /// <response code="201">Loan created successfully.</response>
    /// <response code="404">Equity for the loan not found.</response>
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create(CreateLoanDto createLoanDto)
    {
        try
        {
            var loan = await loanService.CreateLoanAsync(createLoanDto);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

    }

    /// <summary>
    /// Updates an existing loan.
    /// </summary>
    /// <param name="id">The ID of the loan to update.</param>
    /// <param name="updateLoanDto">Updated data for the loan.</param>
    /// <returns>The updated <see cref="LoanDto"/>.</returns>
    /// <response code="200">Loan updated successfully.</response>
    /// <response code="404">Loan not found.</response>
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

    /// <summary>
    /// Deletes a loan by ID.
    /// </summary>
    /// <param name="id">The ID of the loan to delete.</param>
    /// <response code="204">Loan deleted successfully.</response>
    /// <response code="404">Loan not found.</response>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await loanService.DeleteLoanAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}