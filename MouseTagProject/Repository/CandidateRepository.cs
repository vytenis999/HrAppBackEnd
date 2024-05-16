using Azure.Storage.Blobs;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.DTOs.Request;
using MouseTagProject.DTOs.Response;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using MouseTagProject.Services;
using SixLabors.ImageSharp;
using System.Globalization;

namespace MouseTagProject.Repository
{
    public class CandidateRepository : ICandidate
    {
        private readonly MouseTagProjectContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPage _page;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly IAzureStorage _azureStorage;
        public CandidateRepository(MouseTagProjectContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, IPage page, IConfiguration configuration, IAzureStorage azureStorage)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _context = context;
            _page = page;
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
            _azureStorage = azureStorage;

        }

        public async Task<PageDto<CandidateResponseDto>> FilterByWhenWasContactedDate(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var candidateResponseDto = new List<CandidateResponseDto>();

            var recordsNumber = await _context.Candidates.Where(c => c.WhenWasContacted.Max(date => date.Date) >= startDate && c.WhenWasContacted.Max(date => date.Date) <= endDate).CountAsync();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");


            var candidates = await _context.Candidates
                .Where(c => c.WhenWasContacted.Max(date => date.Date) >= startDate && c.WhenWasContacted.Max(date => date.Date) <= endDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Recruiter)
                .Include(c => c.Status)
                .ToListAsync();

            foreach (var candidate in candidates)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(c => c.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }
            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);
            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> FilterByStatus(int statusId, int pageNumber, int pageSize)
        {
            var candidateResponseDto = new List<CandidateResponseDto>();

            var recordsNumber = await _context.Candidates.Where(c => c.Status.Id == statusId).CountAsync();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var candidates = _context.Candidates
                .Where(c => c.Status.Id == statusId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Recruiter)
                .Include(c => c.Status)
                .ToList();

            foreach (var candidate in candidates)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(u => u.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }

            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);

            return result;
        }


        public async Task<PageDto<CandidateResponseDto>> GetCandidatesSorted(int pageNumber, int pageSize, List<Candidate> records)
        {
            var candidateResponseDto = new List<CandidateResponseDto>();

            

            var recordsNumber = await _context.Candidates.CountAsync();

            if (records.Count != recordsNumber)
            {
                recordsNumber = records.Count;
            }


            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var pageremainder = recordsNumber % 20;
            var counter = (pageNumber - 1) * pageSize;
            var returnlist = new List<Candidate>();

            if (pageNumber == pagesResult)
            {
                for (int i = 0; i < pageremainder; i++)
                {
                    var record = records[counter];
                    returnlist.Add(record);
                    counter += 1;
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    var record = records[counter];
                    returnlist.Add(record);
                    counter += 1;
                }
            }

            foreach (var candidate in returnlist)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(u => u.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }

            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListNameDesending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).ToListAsync();
            records.Sort((x, y) => string.Compare(x.Name, y.Name));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListNameAsending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).ToListAsync();
            records.Sort((x, y) => string.Compare(y.Name, x.Name));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListSurNameDesending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).ToListAsync();
            records.Sort((x, y) => string.Compare(x.Surname, y.Surname));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListSurNameAsending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).ToListAsync();
            records.Sort((x, y) => string.Compare(y.Surname, x.Surname));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }
        public async Task<PageDto<CandidateResponseDto>> returnCandidateListWhenContactDesending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).Include(z => z.WhenWasContacted).ToListAsync();
            records.Sort((x, y) => DateTime.Compare(x.WhenWasContacted[x.WhenWasContacted.Count()-1].Date, y.WhenWasContacted[y.WhenWasContacted.Count()-1].Date));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }
        public async Task<PageDto<CandidateResponseDto>> returnCandidateListWhenContactAsending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Include(x => x.Recruiter).Include(c => c.Status).Include(z => z.WhenWasContacted).ToListAsync();
            records.Sort((x, y) => DateTime.Compare(y.WhenWasContacted[y.WhenWasContacted.Count()-1].Date, x.WhenWasContacted[x.WhenWasContacted.Count()-1].Date));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListWillContactAsending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Where(c => c.WillBeContacted != null).Include(x => x.Recruiter).Include(c => c.Status).Include(z => z.WhenWasContacted).ToListAsync();
            records.Sort((x, y) => DateTime.Compare(y.WillBeContacted.Value, x.WillBeContacted.Value));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> returnCandidateListWillContactDesending(int pageNumber, int pageSize)
        {
            var records = await _context.Candidates.Where(c => c.WillBeContacted != null).Include(x => x.Recruiter).Include(c => c.Status).Include(z => z.WhenWasContacted).ToListAsync();
            records.Sort((x, y) => DateTime.Compare(x.WillBeContacted.Value, y.WillBeContacted.Value));
            var result = await GetCandidatesSorted(pageNumber, pageSize, records);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> FilterByTechnologies(int[] technologiesId, int pageNumber, int pageSize)
        {
            var candidateResponseDto = new List<CandidateResponseDto>();

            var c = _context.CandidateTechnologies.GroupBy(ct => ct.CandidateId).Select(t => new { Id = t.Key, Values = t.Select(x => x.TechnologyId) }).ToList();
            var candidatesIds = c.Where(c => technologiesId.All(x => c.Values.Contains(x))).Select(y => y.Id).ToList();

            var recordsNumber = candidatesIds.Count();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var candidates = _context.Candidates
                .Where(c => candidatesIds.Contains(c.Id))
                 .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Recruiter)
                .Include(c => c.Status)
                .ToList();

            foreach (var candidate in candidates)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(u => u.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }

            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> FilterByNameAndSurname(string candidateNameAndSurname, int pageNumber, int pageSize)
        {

            var candidateResponseDto = new List<CandidateResponseDto>();
            var recordsNumber = 0;
            string[] candidateNameSurname = candidateNameAndSurname.Split(' ');
            //var result = new List<CandidateResponseDto>();
            List<Candidate> candidates = new List<Candidate>();
            if (candidateNameSurname.Length == 2)
            {
                recordsNumber = await _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[0]) && c.Surname.Contains(candidateNameSurname[1])).CountAsync();
                candidates = _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[0]) && c.Surname.Contains(candidateNameSurname[1])).Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(x => x.Recruiter).Include(c => c.Status).ToList();
                if (candidates.Count == 0)
                {
                    recordsNumber = await _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[1]) && c.Surname.Contains(candidateNameSurname[0])).CountAsync();
                    candidates = _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[1]) && c.Surname.Contains(candidateNameSurname[0])).Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(x => x.Recruiter).Include(c => c.Status).ToList();
                }
            }
            else
            {
                recordsNumber = await _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[0]) || c.Surname.Contains(candidateNameSurname[0])).CountAsync();
                candidates = _context.Candidates.Where(c => c.Name.Contains(candidateNameSurname[0]) || c.Surname.Contains(candidateNameSurname[0])).Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(x => x.Recruiter).Include(c => c.Status).ToList();
            }

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            foreach (var candidate in candidates)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(u => u.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }

            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);

            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> GetCandidates(int pageNumber, int pageSize)
        {
            var candidateResponseDto = new List<CandidateResponseDto>();

            var recordsNumber = await _context.Candidates.CountAsync();

            if (recordsNumber == 0) throw new InvalidOperationException("No records found");

            var pagesResult = _page.PageSize(recordsNumber, pageSize);

            if (pageNumber <= 0 || pageNumber > pagesResult) throw new InvalidOperationException("Invalid page number");

            var candidates = _context.Candidates
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Recruiter)
            .Include(c => c.Status)
            .ToList();

            foreach (var candidate in candidates)
            {
                var technologies = _context.CandidateTechnologies.Where(ct => ct.CandidateId == candidate.Id).Select(t => t.TechnologyId).ToList();
                var candidateTechnologies = _context.Technologies.Where(t => technologies.Contains(t.Id)).ToList();

                var clients = _context.ClientCandidates.Where(c => c.CandidateId == candidate.Id).Select(c => c.ClientId).ToList();
                var candidateClients = _context.Clients.Where(c => clients.Contains(c.Id)).ToList();

                var userDates = _context.UserDates.Where(c => c.Candidate.Id == candidate.Id).ToList();

                candidateResponseDto.Add(new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : _context.Users.SingleOrDefault(c => c.Id == candidate.Recruiter.Id).UserName,
                    WillBeContacted = candidate.WillBeContacted == null ? null : candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"),
                    Technologies = candidateTechnologies.Select(t => new TechnologyDto { Id = t.Id, TechnologyName = t.TechnologyName }).ToList(),
                    Clients = candidateClients.Select(c => new ClientResponseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Comment = c.Comment
                    }).ToList(),
                    WhenWasContacted = userDates.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                });
            }
            var result = new PageDto<CandidateResponseDto>(pagesResult, pageNumber, recordsNumber, candidateResponseDto);
            return result;
        }

        public async Task<PageDto<CandidateResponseDto>> GetCandidates()
        {
            var candidates = _context.Candidates
                .Include(x => x.WhenWasContacted)
                .Include(x => x.Technologies)
                .ThenInclude(x => x.Technology)
                .Include(c => c.Clients)
                .ThenInclude(c => c.Client)
                .Include(c => c.Recruiter)
                .Include(c => c.Status)
                .ToList()
                .Select(candidate => new CandidateResponseDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    Linkedin = candidate.Linkedin,
                    Comment = candidate.Comment,
                    StatusId = candidate.Status == null ? null : candidate.Status.Id,
                    OtherStatus = candidate.OtherStatus,
                    Salary = candidate.Salary,
                    CVurl = candidate.CVurl,
                    Recruiter = candidate.Recruiter == null ? null : candidate.Recruiter.UserName,
                    WillBeContacted = candidate.WillBeContacted?.ToString("yyyy-MM-dd"),
                    Technologies = candidate.Technologies.Select(x => new TechnologyDto()
                    {
                        Id = x.TechnologyId,
                        TechnologyName = x.Technology.TechnologyName

                    }).ToList(),
                    WhenWasContacted = candidate.WhenWasContacted.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList(),
                    Clients = candidate.Clients.Select(c => new ClientResponseDto()
                    {
                        Id = c.ClientId,
                        Name = c.Client.Name,
                        Comment = c.Client.Comment

                    }).ToList()
                }).ToList();

            var result = new PageDto<CandidateResponseDto>(0, 0, 0, candidates);

            return result;
        }

        public Candidate GetCandidate(int id)
        {
            return _context.Candidates.Where(c => c.Id == id).Include(x => x.WhenWasContacted).Include(x => x.Technologies).ThenInclude(x => x.Technology).Include(c => c.Clients).ThenInclude(c => c.Client).Include(c => c.Status).FirstOrDefault();
        }

        public Candidate GetCandidate(string email)
        {
            return _context.Candidates.FirstOrDefault(c => c.Email == email);
        }

        public List<Candidate> GetCandidatesReminder()
        {
            return _context.Candidates.Include(x => x.WhenWasContacted).ToList();
        }

        public async Task<Candidate> AddCandidate(CandidateRequestDto candidateRequestDto)
        {
            var candidateCheck = _context.Candidates.SingleOrDefault(c => c.Name == candidateRequestDto.Name && c.Surname == candidateRequestDto.Surname && c.Linkedin == candidateRequestDto.Linkedin);

            if (candidateCheck != null) throw new InvalidOperationException($"Candidate already exists {candidateCheck.Name} {candidateCheck.Surname}");

            var contactedDates = new List<UserDate>();

            if (candidateRequestDto.WhenWasContacted != null)
            {
                contactedDates.Add(new UserDate()
                {
                    Date = candidateRequestDto.WhenWasContacted == null ? new DateTime() : DateTime.Parse(candidateRequestDto.WhenWasContacted)
                });
            }

            var candidate = new Candidate
            {
                Name = candidateRequestDto.Name,
                Surname = candidateRequestDto.Surname,
                Email = candidateRequestDto.Email,
                Phone = candidateRequestDto.Phone,
                Linkedin = candidateRequestDto.Linkedin,
                Comment = candidateRequestDto.Comment,
                OtherStatus = candidateRequestDto.OtherStatus,
                Salary = candidateRequestDto.Salary,
                CVurl = candidateRequestDto.CVurl,
                WillBeContacted = candidateRequestDto.WillBeContacted == null ? null : DateTime.Parse(candidateRequestDto.WillBeContacted),
                Technologies = candidateRequestDto.TechnologyIds.Select(t => new CandidateTechnology()
                {
                    TechnologyId = t
                }).ToList(),
                Clients = candidateRequestDto.ClientsIds.Select(c => new ClientCandidate()
                {
                    ClientId = c
                }).ToList(),
                WhenWasContacted = contactedDates
            };

            if (candidateRequestDto.StatusId != null)
            {
                var status = _context.Statuses.FirstOrDefault(s => s.Id == candidateRequestDto.StatusId);
                candidate.Status = status;
            }

            var userId = _currentUserService.GetCurrentUserId();
            var recruiter = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            candidate.Recruiter = recruiter;
            if (candidate.CVurl != null)
            {
                candidate.CVurl = $"CVs/{candidate.Id} {candidate.CVurl}";
            }

            _context.Candidates.Add(candidate);
            _context.SaveChanges();

            return candidate;
        }

        public async Task UploadCandidateFile(int id, IFormFile file)
        {
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            var filename = $"{id} {file.FileName}";

            BlobClient client = container.GetBlobClient(filename);

            await using (Stream? data = file.OpenReadStream())
            {
                await client.UploadAsync(data);
            }
            /*            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }*/

            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == id);
            if (candidate != null)
            {
                candidate.CVurl = filename;
                _context.Update(candidate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task FileUpdate(string Url, int id, IFormFile file)
        {

            await _azureStorage.DeleteAsync(Url);
            await UploadCandidateFile(id, file);

            /*            if (File.Exists(Url))
                        {
                            File.Delete(Url);
                            await UploadCandidateFile(id, file);
                        }
                        else
                        {
                            await UploadCandidateFile(id, file);
                        }*/
        }

        /*        public async Task InitialCandidateFile(string fileURL, IFormFile file)
                {
                    var filename = $"CVs/{fileURL}_{file.FileName}";
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }*/

        public async Task UpdateCandidate(int id, CandidateRequestDto candidateRequestDto)
        {
            var candidate = _context.Candidates.Where(c => c.Linkedin == candidateRequestDto.Linkedin).SingleOrDefault();

            if (candidate.Id != id) throw new InvalidOperationException($"Candidate already exists {candidate.Name} {candidate.Surname}");

            var candidateDate = _context.UserDates.Where(cd => cd.Candidate == candidate).ToList();

            if (candidateRequestDto.StatusId != null)
            {
                var status = _context.Statuses.FirstOrDefault(s => s.Id == candidateRequestDto.StatusId);
                candidate.Status = status;
            }
            else
            {
                candidate.Status = null;
                candidate.OtherStatus = null;
            }

            if (candidateRequestDto.WhenWasContacted != null)
            {
                candidate.WhenWasContacted.Add(new UserDate { Date = DateTime.Parse(candidateRequestDto.WhenWasContacted) });
            }

            if (candidateRequestDto.TechnologyIds != null)
            {
                var candidateTechnologies = _context.CandidateTechnologies.Where(c => c.CandidateId == id).ToList();

                _context.CandidateTechnologies.RemoveRange(candidateTechnologies);

                var newCandidateTechnologies = candidateRequestDto.TechnologyIds.Select(t => { return new CandidateTechnology { CandidateId = id, TechnologyId = t }; }).ToList();

                candidate.Technologies = newCandidateTechnologies;
            }

            if (candidateRequestDto.ClientsIds != null)
            {
                var candidateClients = _context.ClientCandidates.Where(c => c.CandidateId == id).ToList();

                _context.ClientCandidates.RemoveRange(candidateClients);

                var newCandidateClients = candidateRequestDto.ClientsIds.Select(c => { return new ClientCandidate { CandidateId = id, ClientId = c }; }).ToList();

                candidate.Clients = newCandidateClients;
            }

            candidate.Name = candidateRequestDto.Name;
            candidate.Surname = candidateRequestDto.Surname;
            candidate.Email = candidateRequestDto.Email;
            candidate.Phone = candidateRequestDto.Phone;
            candidate.Linkedin = candidateRequestDto.Linkedin;
            candidate.Comment = candidateRequestDto.Comment;
            candidate.OtherStatus = candidateRequestDto.OtherStatus;
            candidate.Salary = candidateRequestDto.Salary;
            candidate.WillBeContacted = candidateRequestDto.WillBeContacted == null ? null : DateTime.Parse(candidateRequestDto.WillBeContacted);

            _context.Candidates.Update(candidate);
            _context.SaveChanges();
        }

        async public void RemoveCandidate(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == id);
            var userDate = _context.UserDates.Where(d => d.Candidate == candidate).ToList();
            if (userDate != null) _context.UserDates.RemoveRange(userDate);
            _context.Candidates.Remove(candidate);
            _context.SaveChanges();
            if (candidate.CVurl != null)
            {
                await _azureStorage.DeleteAsync(candidate.CVurl);
            }
        }

        async public void DeleteCandidates(int[] candidatesIds)
        {
            var userDates = _context.UserDates.Where(us => candidatesIds.Contains(us.Candidate.Id)).ToList();
            if (userDates != null)
            {
                _context.UserDates.RemoveRange(userDates);
            }
            var candidates = _context.Candidates.Where(c => candidatesIds.Contains(c.Id)).ToList();

            var CvUrls = candidates.Select(c => c.CVurl).ToList();

            _context.Candidates.RemoveRange(candidates);
            _context.SaveChanges();

            if (CvUrls != null)
            {
                foreach (var filePath in CvUrls)
                {
                    if (filePath != null) { await _azureStorage.DeleteAsync(filePath); }
                }
            }
        }

        public async Task<bool> CheckLinkedIn(string link)
        {
            if (link != null)
            {
                var candidates = _context.Candidates
                .Select(candidate => new CandidateResponseDto
                {
                    Linkedin = candidate.Linkedin,
                }).ToList();

                bool temp = true;
                for (int i = 0; candidates.Count > i; i++)
                {
                    if (candidates[i].Linkedin == link)
                    {
                        temp = false;
                    }
                }

                return temp;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Candidate>> ImportCandidates(IFormFile file)
        {
            var candidates = await _context.Candidates.CountAsync();
            if (candidates > 800) throw new InvalidOperationException("Candidates already imported");
            var technologies = _context.Technologies.ToList();

            var newfile = new FileInfo(file.FileName);
            var fileExtension = newfile.Extension;

            //Check if file is an Excel File
            if (fileExtension.Contains(".xls"))
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var excelWorkbook = new XLWorkbook(stream))
                    {

                        foreach (var worksheet in excelWorkbook.Worksheets)
                        {
                            var headerRow = true;
                            var usedRows = worksheet.RowsUsed();
                            foreach (var row in usedRows)
                            {
                                if (headerRow)
                                    headerRow = false;
                                else
                                {
                                    try
                                    {
                                        var newCandidateTechnologies = new List<CandidateTechnology>();
                                        var newCandidateDates = new List<UserDate>();
                                        var cultureInfo = new CultureInfo("de-DE");

                                        var technologyName = row.Cell(6).ToString() == string.Empty ? string.Empty : row.Cell(6).Value.ToString();
                                        var newStatus = row.Cell(8).ToString() == string.Empty ? string.Empty : row.Cell(8).Value.ToString();
                                        var newRecruter = row.Cell(12).ToString() == string.Empty ? null : row.Cell(12).Value.ToString();
                                        var splittedTechnology = technologyName.Split(",");
                                        var finalTechnology = new List<string>();

                                        foreach (var item in splittedTechnology)
                                        {
                                            var splitN = item.Split("\n");
                                            finalTechnology.AddRange(splitN);
                                        }

                                        var newCandidate = new Candidate
                                        {
                                            Name = row.Cell(2).ToString() == string.Empty ? null : row.Cell(2).Value.ToString(),
                                            Surname = row.Cell(3).ToString() == string.Empty ? null : row.Cell(3).Value.ToString(),
                                            Email = row.Cell(9).ToString() == string.Empty ? null : row.Cell(9).Value.ToString(),
                                            Phone = row.Cell(10).ToString() == string.Empty ? null : row.Cell(10).Value.ToString(),
                                            Linkedin = row.Cell(4).ToString() == string.Empty ? null : row.Cell(4).Value.ToString(),
                                            Comment = row.Cell(5).ToString() == string.Empty ? null : row.Cell(5).Value.ToString(),
                                            OtherStatus = null,
                                            Salary = row.Cell(11).ToString() == string.Empty ? null : row.Cell(11).Value.ToString(),
                                            CVurl = null,
                                            WillBeContacted = row.Cell(7).Value.ToString() == string.Empty ? null : DateTime.Parse(row.Cell(7).Value.ToString(), CultureInfo.CurrentCulture),
                                            Technologies = newCandidateTechnologies,
                                            Clients = null,
                                            WhenWasContacted = newCandidateDates,
                                            Recruiter = _context.Users.Where(x => x.UserName.Equals(newRecruter)).FirstOrDefault(),
                                            Status = _context.Statuses.Where(x => x.Value.Equals(newStatus)).FirstOrDefault(),
                                        };


                                        if (row.Cell(1).Value.ToString().Length == 0)
                                        {
                                            newCandidateDates = null;
                                        }
                                        else if (row.Cell(1).Value.ToString().Length <= 5)
                                        {
                                            newCandidateDates.Add(new UserDate { Date = DateTime.Parse(row.Cell(1).Value.ToString(), cultureInfo), Candidate = newCandidate });
                                        }
                                        else
                                        {
                                            var splittedDates = row.Cell(1).Value.ToString().Split("\n");
                                            foreach (var date in splittedDates)
                                            {
                                                var newDate = DateTime.Parse(date, CultureInfo.CurrentCulture);
                                                newCandidateDates.Add(new UserDate
                                                {
                                                    Date = newDate,
                                                });
                                            }
                                        }

                                        foreach (var t in finalTechnology)
                                        {
                                            if (t != "")
                                            {
                                                var tUpper = t.ToUpper();
                                                string trimmed = String.Concat(tUpper.Where(c => !Char.IsWhiteSpace(c)));
                                                var getTechnology = technologies.FirstOrDefault(x => x.TechnologyName == trimmed);
                                                if (getTechnology != null)
                                                {
                                                    newCandidateTechnologies.Add(new CandidateTechnology
                                                    {
                                                        CandidateId = newCandidate.Id,
                                                        TechnologyId = getTechnology.Id,
                                                    });
                                                }
                                            }
                                        }

                                        await _context.AddAsync(newCandidate);
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException(ex.Message);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return new List<Candidate>();
        }
    }
}
