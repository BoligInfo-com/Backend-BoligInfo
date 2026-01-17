using BoligInfo.CashService;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashController;

/// <summary>
/// Controller for managing cash entities.
/// Provides endpoints to retrieve, create, update, and delete <see cref="CashDto"/> objects.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AllCashController(
    ICashService cashService, 
    ILogger<AllCashController> logger
    ) : ControllerBase, IAllCashController
{
    
    /// <summary>
    /// Retrieves all cash records.
    /// </summary>
    /// <returns>List of <see cref="CashDto"/> objects.</returns>
    /// <response code="200">Returns all cash records.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CashDto>>> GetAll()
    {
        var allCash = await cashService.GetAllCashAsync();
        return Ok(allCash);
    }

    /// <summary>
    /// Retrieves a cash record by its ID.
    /// </summary>
    /// <param name="id">The ID of the cash record.</param>
    /// <returns>The <see cref="CashDto"/> with the specified ID.</returns>
    /// <response code="200">Cash record found and returned.</response>
    /// <response code="404">Cash record not found.</response>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CashDto>> GetById(long id)
    {
        var cash = await cashService.GetCashByIdAsync(id);
        if (cash == null)
            return NotFound();
        
        return Ok(cash);
    }

    /// <summary>
    /// Retrieves all cash records associated with a specific equity.
    /// </summary>
    /// <param name="equityId">The ID of the equity.</param>
    /// <returns>List of cash records for the given equity.</returns>
    /// <response code="200">Returns cash records for the equity.</response>
    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<CashDto>>> GetByEquityId(long equityId)
    {
        var allCash = await cashService.GetAllCashByEquityIdAsync(equityId);
        return Ok(allCash);
    }

    /// <summary>
    /// Creates a new cash record.
    /// </summary>
    /// <param name="createCashDto">Data for the new cash record.</param>
    /// <returns>The created <see cref="CashDto"/> object.</returns>
    /// <response code="201">Cash record created successfully.</response>
    /// <response code="404">Parent equity not found.</response>
    /// <response code="400">Equity already has a cash record (one-to-one constraint).</response>
    [HttpPost]
    public async Task<ActionResult<CashDto>> Create(CreateCashDto createCashDto)
    {
        try
        {
            var cash = await cashService.CreateCashAsync(createCashDto);
            return CreatedAtAction(nameof(GetById), new { id = cash.Id }, cash);
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e.Message);
            return NotFound();
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e.Message);
            return BadRequest();
        }
    }

    /// <summary>
    /// Updates an existing cash record.
    /// </summary>
    /// <param name="id">The ID of the cash record to update.</param>
    /// <param name="updateCashDto">Updated data for the cash record.</param>
    /// <returns>The updated <see cref="CashDto"/> object.</returns>
    /// <response code="200">Cash record updated successfully.</response>
    /// <response code="404">Cash record not found.</response>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<CashDto>> Update(long id, UpdateCashDto updateCashDto)
    {
        try
        {
            var cash = await cashService.UpdateCashAsync(id, updateCashDto);
            return Ok(cash);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a cash record by ID.
    /// </summary>
    /// <param name="id">The ID of the cash record to delete.</param>
    /// <response code="204">Cash record deleted successfully.</response>
    /// <response code="404">Cash record not found.</response>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await cashService.DeleteCashAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}