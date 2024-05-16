using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.DTOs.ClientDtos;
using MouseTagProject.DTOs.Request;
using MouseTagProject.Models;
using MouseTagProject.Repository;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly ClientRepository _clientRepository;

        public ClientController(ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var clients = await _clientRepository.GetClientsAsync(pageNumber, pageSize);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetClientList")]
        public async Task<IActionResult> GetClientsList()
        {
            var clients = await _clientRepository.GetClientsListAsync();
            if (clients.Count == 0)
            {
                return NotFound();
            }

            return Ok(clients);
        }

        [HttpGet("filter/{clientName}")]
        public async Task<IActionResult> FilterByClient(string clientName,[FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            if (clientName == null || clientName == string.Empty) return BadRequest();
            try
            {
                var result = await _clientRepository.FilterByClientName(clientName, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _clientRepository.GetClientAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] ClientRequestDto clientRequestDto)
        {
            try
            {
                var client = await _clientRepository.AddClientAsync(clientRequestDto);

                var result = new ClientGetDto(client.Id, client.Name,client.Project, client.Comment, client.WillBeContacted == null? null : client.WillBeContacted.Value.ToString("yyyy-MM-dd"), null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientRequestDto clientRequestDto)
        {
            try
            {
                var client = await _clientRepository.UpdateClientAsync(id, clientRequestDto);

                var result = new ClientGetDto(client.Id, client.Name, client.Project, client.Comment, client.WillBeContacted == null ? null : client.WillBeContacted.Value.ToString("yyyy-MM-dd"), null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin"), Route("{id}")]
        public async Task<IActionResult> RemoveClient(int id)
        {
            await _clientRepository.RemoveClientAsync(id);
            return Ok();
        }

        [Authorize(Roles = "Admin, SuperAdmin"), HttpDelete("{clientId}/{candidateId}")]
        public async Task<IActionResult> RemoveClientCandidate(int clientId, int candidateId)
        {
            await _clientRepository.RemoveClientCandidate(clientId, candidateId);
            return Ok();
        }

        [Authorize(Roles = "Admin, SuperAdmin"), HttpPost("delete/clients")]
        public async Task<IActionResult> RemoveClients([FromBody] int[] clientsIds)
        {
            await _clientRepository.RemoveClients(clientsIds);
            return Ok();
        }

    }
}
