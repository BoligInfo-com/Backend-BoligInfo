using BoligInfo.Core.DTO;
using BoligInfo.HouseService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.HouseController;

/// <summary>
/// Controller for managing houses.
/// Provides endpoints to retrieve, create, update, and delete <see cref="HouseDto"/> entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HousesController(IHouseService houseService) : ControllerBase, IHousesController
{
    /// <summary>
    /// Retrieves all houses.
    /// </summary>
    /// <returns>A list of <see cref="HouseDto"/> objects.</returns>
    /// <response code="200">Returns all loans.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HouseDto>>> GetAll()
    {
        var houses = await houseService.GetAllHousesAsync();
        return Ok(houses);
    }

    /// <summary>
    /// Retrieves a house by ID.
    /// </summary>
    /// <param name="id">The ID of the loan.</param>
    /// <returns>The <see cref="HouseDto"/> with the specified ID.</returns>
    /// <response code="200">Returns the loan.</response>
    /// <response code="404">Loan not found.</response>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<HouseDto>> GetById(long id)
    {
        var house = await houseService.GetHouseByIdAsync(id);
        if (house == null)
            return NotFound();
        
        return Ok(house);
    }

    /// <summary>
    /// Retrieves all houses associated with a specific equity.
    /// </summary>
    /// <param name="equityId">The ID of the equity.</param>
    /// <returns>List of houses for the given equity.</returns>
    /// <response code="200">Returns houses for the equity.</response>
    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<HouseDto>>> GetByEquityId(long equityId)
    {
        var houses = await houseService.GetHousesByEquityIdAsync(equityId);
        return Ok(houses);
    }

    /// <summary>
    /// Retrieves a house by ID along with all associated cashFlows.
    /// </summary>
    /// <param name="id">The ID of the house.</param>
    /// <returns>The <see cref="HouseDto"/> with cashFlows included.</returns>
    /// <response code="200">House with cashFlows returned.</response>
    /// <response code="404">House not found.</response>
    [HttpGet("{id:long}/with-cashflows")]
    public async Task<ActionResult<HouseDto>> GetByIdWithCashFlows(long id)
    {
        var house = await houseService.GetHouseWithCashFlowsAsync(id);
        if (house == null)
            return NotFound();
        
        return Ok(house);
    }

    /// <summary>
    /// Creates a new loan.
    /// </summary>
    /// <param name="createHouseDto">Data for the new house.</param>
    /// <returns>The created <see cref="HouseDto"/>.</returns>
    /// <response code="201">House created successfully.</response>
    /// <response code="404">Equity for the house not found.</response>
    [HttpPost]
    public async Task<ActionResult<HouseDto>> Create(CreateHouseDto createHouseDto)
    {
        try
        {
            var house = await houseService.CreateHouseAsync(createHouseDto);
            return CreatedAtAction(nameof(GetById), new { id = house.Id }, house);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Updates an existing house.
    /// </summary>
    /// <param name="id">The ID of the house to update.</param>
    /// <param name="updateHouseDto">Updated data for the house.</param>
    /// <returns>The updated <see cref="HouseDto"/>.</returns>
    /// <response code="200">House updated successfully.</response>
    /// <response code="404">House not found.</response>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<HouseDto>> Update(long id, UpdateHouseDto updateHouseDto)
    {
        try
        {
            var house = await houseService.UpdateHouseAsync(id, updateHouseDto);
            return Ok(house);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a house by ID.
    /// </summary>
    /// <param name="id">The ID of the house to delete.</param>
    /// <response code="204">House deleted successfully.</response>
    /// <response code="404">House not found.</response>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await houseService.DeleteHouseAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}