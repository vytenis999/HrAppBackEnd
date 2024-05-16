using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.Interfaces;
using MouseTagProject.Services;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OfferController : ControllerBase
    {
        private readonly ICandidate _candidate;
        private readonly OfferService _offerService;

        public OfferController(ICandidate candidate, OfferService offerService)
        {
            _candidate = candidate;
            _offerService = offerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GenerateOffer(int id)
        {
            var candidate = _candidate.GetCandidate(id);

            var filename = $"{candidate.Name.Trim()}_{candidate.Surname.Trim()}.docx";

            var bytes = await _offerService.CreateOffer(candidate);

            // Change the MIME type to 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
        }
    }
}
