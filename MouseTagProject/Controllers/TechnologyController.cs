using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TechnologyController : ControllerBase
    {
        private readonly ITechnology _technologyRepository;

        public TechnologyController(ITechnology technologyRepository)
        {
            _technologyRepository = technologyRepository;
        }

        [HttpGet]
        public IActionResult GetTechnologies()
        {
            var technologies = _technologyRepository.GetTechnologies();
            if (technologies.Count == 0)
            {
                return NotFound();
            }
            return Ok(technologies);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTechnology(int id)
        {
            var technology = _technologyRepository.GetTechnology(id);
            if (technology == null)
            {
                return NotFound();
            }
            return Ok(technology);
        }

        [HttpPost]
        public IActionResult AddTechnology(TechnologyDto technologyDto)
        {
            Technology technology = new Technology()
            {
                TechnologyName = technologyDto.TechnologyName
            };

            _technologyRepository.AddTechnology(technology);
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateTechnology(int id, TechnologyDto technologyDto)
        {
            Technology technology = new Technology()
            {
                TechnologyName = technologyDto.TechnologyName
            };

            _technologyRepository.UpdateTechnology(id, technology);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveTechnology(int id)
        {
            _technologyRepository.RemoveTechnology(id);
            return Ok();
        }
    }
}
