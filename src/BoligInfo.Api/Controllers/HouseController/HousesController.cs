using BoligInfo.Core.DTO;
using BoligInfo.HouseService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.HouseController;

[ApiController]
[Route("api/[controller]")]
public class HousesController(IHouseService houseService) : ControllerBase, IHousesController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HouseDto>>> GetAll()
    {
        var houses = await houseService.GetAllHousesAsync();
        return Ok(houses);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<HouseDto>> GetById(long id)
    {
        var house = await houseService.GetHouseByIdAsync(id);
        if (house == null)
            return NotFound();
        
        return Ok(house);
    }

    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<HouseDto>>> GetByEquityId(long equityId)
    {
        var houses = await houseService.GetHousesByEquityIdAsync(equityId);
        return Ok(houses);
    }

    [HttpGet("{id:long}/with-cashflows")]
    public async Task<ActionResult<HouseDto>> GetByIdWithCashFlows(long id)
    {
        var house = await houseService.GetHouseWithCashFlowsAsync(id);
        if (house == null)
            return NotFound();
        
        return Ok(house);
    }

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