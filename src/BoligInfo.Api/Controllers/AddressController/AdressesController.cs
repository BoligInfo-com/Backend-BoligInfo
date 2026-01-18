using BoligInfo.AddressService;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.AddressController;

[ApiController]
[Route("api/[controller]")]
public class AddressesController(
    IAddressService addressService,
    ILogger<AddressesController> logger) : ControllerBase, IAddressesController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
    {
        var addresses = await addressService.GetAllAddressesAsync();
        return Ok(addresses);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AddressDto>> GetById(long id)
    {
        var address = await addressService.GetAddressByIdAsync(id);
        if (address == null)
            return NotFound();
        
        return Ok(address);
    }

    [HttpGet("house/{houseId:long}")]
    public async Task<ActionResult<AddressDto>> GetByHouseId(long houseId)
    {
        var address = await addressService.GetAddressByHouseIdAsync(houseId);
        if (address == null)
            return NotFound();
        
        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create(CreateAddressDto createAddressDto)
    {
        try
        {
            var address = await addressService.CreateAddressAsync(createAddressDto);
            return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AddressDto>> Update(long id, UpdateAddressDto updateAddressDto)
    {
        try
        {
            var address = await addressService.UpdateAddressAsync(id, updateAddressDto);
            return Ok(address);
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
            await addressService.DeleteAddressAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}