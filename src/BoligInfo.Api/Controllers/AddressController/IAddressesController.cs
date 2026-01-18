using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.AddressController;

public interface IAddressesController
{
    Task<ActionResult<IEnumerable<AddressDto>>> GetAll();
    Task<ActionResult<AddressDto>> GetById(long id);
    Task<ActionResult<AddressDto>> GetByHouseId(long houseId);
    Task<ActionResult<AddressDto>> Create(CreateAddressDto createAddressDto);
    Task<ActionResult<AddressDto>> Update(long id, UpdateAddressDto updateAddressDto);
    Task<ActionResult> Delete(long id);
}