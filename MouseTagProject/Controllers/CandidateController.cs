using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.DTOs.Request;
using MouseTagProject.DTOs.Response;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using MouseTagProject.Services;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]

    public class CandidateController : ControllerBase
    {
        private readonly ICandidate _candidateRepository;
        private readonly ILinkedInService _linkedInService;
        private readonly MouseTagProjectContext _context;
        private readonly IAzureStorage _storage;

        public CandidateController(ICandidate candidateRepository, ILinkedInService linkedInService, MouseTagProjectContext context, IAzureStorage storage)
        {
            _candidateRepository = candidateRepository;
            _linkedInService = linkedInService;
            _context = context;
            _storage = storage;
        }

        [HttpGet("filter/date/{startDate}/{endDate}")]
        public async Task<IActionResult> FilterByDate( DateTime startDate, DateTime endDate, [FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.FilterByWhenWasContactedDate(startDate, endDate, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("filter/status/{statusId}")]
        public async Task<IActionResult> FilterByStatus(int statusId, [FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.FilterByStatus(statusId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("filter/technologies")]
        public async Task<IActionResult> FilterByTechnologies([FromBody] int[] technologiesIds, [FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.FilterByTechnologies(technologiesIds, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("filter/NameAndSurname/{candidate}")]
        public async Task<IActionResult> FilterByNameAndSurname(string candidate, [FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.FilterByNameAndSurname(candidate, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Import")]
        public async Task<IActionResult> ImportCandidates()
        {
            try
            {
                var file = Request.Form.Files[0];
                await _candidateRepository.ImportCandidates(file);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DownloadCandidatesListFile")]
        public async Task<IActionResult> DownloadFileAsync()
        {
            string fileName = "Candidates.xlsx";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Candidates");
                worksheet.Cell(1, 1).Value = "Kada buvo susisiekta";
                worksheet.Cell(1, 2).Value = "Vardas";
                worksheet.Cell(1, 3).Value = "Pavardė";
                worksheet.Cell(1, 4).Value = "LinkedIn nuoroda";
                worksheet.Cell(1, 5).Value = "Komentaras";
                worksheet.Cell(1, 6).Value = "Technologijos";
                worksheet.Cell(1, 7).Value = "Kada reikia susiekti";
                worksheet.Cell(1, 8).Value = "Kandidato statusas";
                worksheet.Cell(1, 9).Value = "El. paštas";
                worksheet.Cell(1, 10).Value = "Telefonas";
                worksheet.Cell(1, 11).Value = "Atlyginimas";
                worksheet.Cell(1, 12).Value = "Rekruteris";
                worksheet.Cell(1, 13).Value = "Klientai";
                var candidates = await _candidateRepository.GetCandidates();
                int count = 1;
                foreach (var candidate in candidates.data)
                {
                    string status = string.Empty;
                    if (candidate.StatusId <= 3)
                    {
                        status = _context.Statuses.ToList()[(int)candidate.StatusId].Value;
                    }
                    else
                    {
                        status = candidate.OtherStatus;
                    }

                    count += 1;
                    worksheet.Cell(count, 1).Value = string.Join("\n", candidate.WhenWasContacted.Select(x => string.Format("{0:d}", x)).ToList());
                    worksheet.Cell(count, 2).Value = candidate.Name;
                    worksheet.Cell(count, 3).Value = candidate.Surname;
                    worksheet.Cell(count, 4).Value = candidate.Linkedin;
                    worksheet.Cell(count, 5).Value = candidate.Comment;
                    worksheet.Cell(count, 6).Value = string.Join(",", candidate.Technologies.Select(x => x.TechnologyName).ToList());
                    worksheet.Cell(count, 7).Value = string.Format("{0:d}", candidate.WillBeContacted);
                    worksheet.Cell(count, 8).Value = status;
                    worksheet.Cell(count, 9).Value = candidate.Email;
                    worksheet.Cell(count, 10).Value = candidate.Phone;
                    worksheet.Cell(count, 11).Value = candidate.Salary;
                    worksheet.Cell(count, 12).Value = candidate.Recruiter;
                    worksheet.Cell(count, 13).Value = string.Join(",", candidate.Clients.Select(x => x.Name).ToList());
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/octet-stream", fileName);
                }
            }
        }


        [HttpGet("DownloadCandidateCV/{url}")]
        public async Task<IActionResult> DownloadCVAsync(string url)
        {

            BlobDto? file = await _storage.DownloadAsync(url);

            // Check if file was found
            if (file == null)
            {
                // Was not, return error message to client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {url} could not be downloaded.");
            }
            else
            {
                // File was found, return it to client
                return File(file.Content, file.ContentType, file.Name);
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetCandidates([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.GetCandidates(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("returnCandidateListNameDesending")]
        public async Task<IActionResult> returnCandidateListNameDesending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListNameDesending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListNameAsending")]
        public async Task<IActionResult> returnCandidateListNameAsending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListNameAsending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListSurNameDesending")]
        public async Task<IActionResult> returnCandidateListSurNameDesending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListSurNameDesending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListSurNameAsending")]
        public async Task<IActionResult> returnCandidateListSurNameAsending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListSurNameAsending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

 
        [HttpGet]
        [Route("returnCandidateListWhenContactAsending")]
        public async Task<IActionResult> returnCandidateListWhenContactAsending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListWhenContactAsending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListWhenContactDesending")]
        public async Task<IActionResult> returnCandidateListWhenContactDesending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListWhenContactDesending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListWillContactAsending")]
        public async Task<IActionResult> returnCandidateListWillContactAsending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListWillContactAsending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("returnCandidateListWillContactDesending")]
        public async Task<IActionResult> returnCandidateListWillContactDesending([FromQuery] int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var result = await _candidateRepository.returnCandidateListWillContactDesending(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult GetCandidate(int id)
        {
            var candidate = _candidateRepository.GetCandidate(id);
            if (candidate == null)
            {
                return NotFound();
            }

            CandidateResponseDto candidateResponseDto = new CandidateResponseDto()
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
                    Id = c.Id,
                    Name = c.Client.Name,
                    Comment = c.Client.Comment

                }).ToList()
            };

            return Ok(candidateResponseDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddCandidate([FromForm] CandidateCreateDto candidateCreateDto)
        {
            try
            {
                CandidateRequestDto candidateRequestDto = new CandidateRequestDto();
                candidateRequestDto.Name = candidateCreateDto.Name;
                candidateRequestDto.Surname = candidateCreateDto.Surname;
                candidateRequestDto.Email = candidateCreateDto.Email;
                candidateRequestDto.Phone = candidateCreateDto.Phone;
                candidateRequestDto.Linkedin = candidateCreateDto.Linkedin;
                candidateRequestDto.Comment = candidateCreateDto.Comment;
                candidateRequestDto.StatusId = candidateCreateDto.StatusId;
                candidateRequestDto.OtherStatus = candidateCreateDto.OtherStatus;
                candidateRequestDto.Salary = candidateCreateDto.Salary;
                if (candidateCreateDto.file != null)
                {
                    candidateRequestDto.CVurl = candidateCreateDto.file.FileName;
                }
                candidateRequestDto.WillBeContacted = candidateCreateDto.WillBeContacted;
                if (candidateCreateDto.TechnologyIds != null)
                {
                    candidateRequestDto.TechnologyIds = candidateCreateDto.TechnologyIds.Split(',')?.Select(Int32.Parse)?.ToList();
                }
                if (candidateCreateDto.ClientsIds != null)
                {
                    candidateRequestDto.ClientsIds = candidateCreateDto.ClientsIds.Split(',')?.Select(Int32.Parse)?.ToList();
                }
                candidateRequestDto.WhenWasContacted = candidateCreateDto.WhenWasContacted;

                var candidate = await _candidateRepository.AddCandidate(candidateRequestDto);

                if (candidateCreateDto.file != null)
                {
                    await _candidateRepository.UploadCandidateFile(candidate.Id, candidateCreateDto.file);
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return BadRequest(e.Message);
            }

        }

        [HttpPatch]
        [Route("FileUpdate")]
        public async Task<IActionResult> FileUpdate(FileUploadDto fileUploadDto)
        {
            try
            {
                if (Url != null)
                {
                    await _candidateRepository.FileUpdate(fileUploadDto.CVurl, Int32.Parse(fileUploadDto.Id), fileUploadDto.file);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCandidate(int id, [FromBody] CandidateRequestDto candidateRequestDto)
        {
            try
            {
                await _candidateRepository.UpdateCandidate(id, candidateRequestDto);
                if (candidateRequestDto.file != null)
                {
                    if (candidateRequestDto.CVurl != null)
                    {
                        _candidateRepository.FileUpdate(candidateRequestDto.CVurl, id, candidateRequestDto.file);
                    }
                    else
                    {
                        _candidateRepository.UploadCandidateFile(id, candidateRequestDto.file);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin"), Route("{id}")]
        public IActionResult RemoveCandidate(int id)
        {
            _candidateRepository.RemoveCandidate(id);
            return Ok();
        }


        [HttpPost]
        [Route("LinkedIn")]
        public IActionResult GetDataLinkedIn([FromBody] string link)
        {
            try
            {
                var result = _linkedInService.ScrapeSerp(link);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CheckLinkedIn")]
        public async Task<bool> CheckLinkedIn([FromBody] string link)
        {
            try
            {
                var result = await _candidateRepository.CheckLinkedIn(link);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin"),HttpPost("multidelete")]
        public IActionResult DeleteCandidates([FromBody] int[] candidatesIds)
        {
            _candidateRepository.DeleteCandidates(candidatesIds);
            return Ok();
        }

    }
}
