using BoligInfo.CashService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashController;

[ApiController]
[Route("api/[controller]")]
public class AllCashController(ICashService cashService) : ControllerBase, IAllCashController
{
    
}