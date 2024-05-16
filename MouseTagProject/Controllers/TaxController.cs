using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.DTOs;
using MouseTagProject.Services;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaxController : ControllerBase
    {
        private readonly TaxService _taxService;

        public TaxController(TaxService taxService)
        {
            _taxService = taxService;
        }

        [HttpPost]
        public async Task<IActionResult> TaxReturn([FromBody] TaxInputDto TaxInputDto)
        {
            var tax_return = await _taxService.Get(TaxInputDto);
            return Ok(tax_return);
        }
    }
}
