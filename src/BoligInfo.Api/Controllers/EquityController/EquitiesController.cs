using BoligInfo.Core.DTO;
using BoligInfo.EquityService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.EquityController;

/// <summary>
/// Controller for managing equity entities.
/// Provides endpoints to retrieve, create, update, and delete <see cref="EquityDto"/> objects.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EquitiesController(IEquityService equityService) : ControllerBase, IEquitiesController
{
    
    /// <summary>
    /// Retrieves all equities.
    /// </summary>
    /// <returns>List of <see cref="EquityDto"/> objects.</returns>
    /// <response code="200">Returns all equities.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquityDto>>> GetAll()
    {
        var equities = await equityService.GetAllEquitiesAsync();
        return Ok(equities);
    }

    /// <summary>
    /// Retrieves an equity by ID.
    /// </summary>
    /// <param name="id">The ID of the equity.</param>
    /// <returns>The <see cref="EquityDto"/> with the specified ID.</returns>
    /// <response code="200">Equity found and returned.</response>
    /// <response code="404">Equity not found.</response>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EquityDto>> GetById(long id)
    {
        var equity = await equityService.GetEquityByIdAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    /// <summary>
    /// Retrieves an equity by ID along with all associated loans.
    /// </summary>
    /// <param name="id">The ID of the equity.</param>
    /// <returns>The <see cref="EquityDto"/> with loans included.</returns>
    /// <response code="200">Equity with loans returned.</response>
    /// <response code="404">Equity not found.</response>
    [HttpGet("{id:long}/with-loans")]
    public async Task<ActionResult<EquityDto>> GetByIdWithLoans(long id)
    {
        var equity = await equityService.GetEquityWithLoansAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    /// <summary>
    /// Creates a new equity.
    /// </summary>
    /// <param name="createEquityDto">Data for the new equity.</param>
    /// <returns>The created <see cref="EquityDto"/> object.</returns>
    /// <response code="201">Equity created successfully.</response>
    /// <response code="404">Equity creation failed due to missing references.</response>
    [HttpPost]
    public async Task<ActionResult<EquityDto>> Create(CreateEquityDto createEquityDto)
    {
        try
        {
            var equity = await equityService.CreateEquityAsync(createEquityDto);
            return CreatedAtAction(nameof(GetById), new { id = equity.Id }, equity);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

    }
    
    /// <summary>
    /// Updates an existing equity.
    /// </summary>
    /// <param name="id">The ID of the equity to update.</param>
    /// <param name="updateEquityDto">Updated equity data.</param>
    /// <returns>The updated <see cref="EquityDto"/> object.</returns>
    /// <response code="200">Equity updated successfully.</response>
    /// <response code="404">Equity not found.</response>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<EquityDto>> Update(long id, UpdateEquityDto updateEquityDto)
    {
        try
        {
            var equity = await equityService.UpdateEquityAsync(id, updateEquityDto);
            return Ok(equity);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes an equity by ID.
    /// </summary>
    /// <param name="id">The ID of the equity to delete.</param>
    /// <response code="204">Equity deleted successfully.</response>
    /// <response code="404">Equity not found.</response>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await equityService.DeleteEquityAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();   
        }
    }
}