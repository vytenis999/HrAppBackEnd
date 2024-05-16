using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.Interfaces;
using MouseTagProject.Repository;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendar _calendarRepository;

        public CalendarController(ICalendar calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }
        [HttpGet("getCandidates/{date}")]
        public async Task<IActionResult> GetIdentityUsers(string date)
        {
            var candidates =  _calendarRepository.GetCandidates(date);
            if (candidates.Count == 0)
            {
                return NotFound();
            }

            return Ok(candidates);
        }
    }
}
