using Microsoft.EntityFrameworkCore;
using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.DTOs.ClientDtos;
using MouseTagProject.DTOs.Request;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;

namespace MouseTagProject.Repository
{
    public class ClientRepository
    {
        private readonly MouseTagProjectContext _context;
        private readonly IPage _page;

        public ClientRepository(MouseTagProjectContext context, IPage page)
        {
            _context = context;
            _page = page;
        }

        public async Task<PageDto<ClientGetDto>> GetClientsAsync(int pageNumber, int pageSize)
        {
            var recordsNumber = await _context.Clients.CountAsync();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var clientDto = _context.Clients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Candidates)
                .ThenInclude(c => c.Candidate)
                .ThenInclude(c => c.Technologies)
                .ThenInclude(c => c.Technology)
                .ToList()
                .Select(client => new ClientGetDto
                    (
                        client.Id,
                        client.Name,
                        client.Project,
                        client.Comment,
                        client.WillBeContacted == null ? null : client.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                        client.Candidates.Select(c => new ClientCandidateDto
                    (c.CandidateId,
                        c.Candidate.Name,
                        c.Candidate.Surname,
                        c.Candidate.Email,
                        c.Candidate.Technologies.Select(t => new TechnologyDto { Id = t.Technology.Id, TechnologyName = t.Technology.TechnologyName })
                    ))))
                .ToList();

            var result = new PageDto<ClientGetDto>(pagesResult, pageNumber, recordsNumber, clientDto);

            return result;
        }

        public async Task<PageDto<ClientGetDto>> FilterByClientName(string name, int pageNumber, int pageSize)
        {
            var recordsNumber = await _context.Clients.Where(c => c.Name.Contains(name)).CountAsync();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var clientDto = _context.Clients
                .Where(c => c.Name.Contains(name))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Candidates)
                .ThenInclude(c => c.Candidate)
                .ThenInclude(c => c.Technologies)
                .ThenInclude(c => c.Technology)
                .ToList()
                .Select(client => new ClientGetDto
                    (
                        client.Id,
                        client.Name,
                        client.Project,
                        client.Comment,
                        client.WillBeContacted == null ? null : client.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                        client.Candidates.Select(c => new ClientCandidateDto
                    (c.CandidateId,
                        c.Candidate.Name,
                        c.Candidate.Surname,
                        c.Candidate.Email,
                        c.Candidate.Technologies.Select(t => new TechnologyDto { Id = t.Technology.Id, TechnologyName = t.Technology.TechnologyName })
                    ))))
                .ToList();

            var result = new PageDto<ClientGetDto>(pagesResult, pageNumber, recordsNumber, clientDto);

            return result;
        }

        public async Task<List<GetClientListDto>> GetClientsListAsync()
        {
            var result = _context.Clients
                .Include(c => c.Candidates)
                .ToList()
                .Select(client => new GetClientListDto
                    (
                        client.Id,
                        client.Name,
                        client.Comment
                    ))
                .ToList();

            return result;
        }

        public async Task<Client> GetClientAsync(int id)
        {
            return await _context.Clients.Include(c => c.Candidates).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> AddClientAsync(ClientRequestDto clientRequestDto)
        {
            var client = _context.Clients.SingleOrDefault(c => c.Normalized == clientRequestDto.Name.ToUpper());

            if (client != null) throw new InvalidOperationException($"Client {clientRequestDto.Name} already exists");

            var newClient = new Client 
            {
                Name = clientRequestDto.Name,
                Normalized = clientRequestDto.Name.ToUpper(),
                Project = clientRequestDto.Project,
                Comment = clientRequestDto.Comment,
                WillBeContacted = clientRequestDto.WillBeContacted == null ? null : DateTime.Parse(clientRequestDto.WillBeContacted)
            };

            await _context.Clients.AddAsync(newClient);
            await _context.SaveChangesAsync();
            return newClient;
        }

        public async Task<Client> UpdateClientAsync(int id, ClientRequestDto clientRequestDto)
        {

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

            client.Name = clientRequestDto.Name;
            client.Project = clientRequestDto.Project;
            client.Comment = clientRequestDto.Comment;
            client.WillBeContacted = clientRequestDto.WillBeContacted == null ? null : DateTime.Parse(clientRequestDto.WillBeContacted);
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task RemoveClientAsync(int id)
        {
            var clientCandidates = _context.ClientCandidates.Where(ct => ct.ClientId == id).ToList();
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            _context.ClientCandidates.RemoveRange(clientCandidates);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveClientCandidate(int clientId, int candidateId)
        {
            var result = _context.ClientCandidates.FirstOrDefault(ct => ct.ClientId == clientId && ct.CandidateId == candidateId);
            if (result != null)
            {
                _context.ClientCandidates.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveClients(int[] clientsIds)
        {
            var clientCandidates = _context.ClientCandidates.Where(ct => clientsIds.Contains(ct.ClientId)).ToList();
            var clients = _context.Clients.Where(c => clientsIds.Contains(c.Id)).ToList();

            _context.ClientCandidates.RemoveRange(clientCandidates);
            _context.Clients.RemoveRange(clients);

            await _context.SaveChangesAsync();
        }
    }
}
