using MouseTagProject.DTOs;
using MouseTagProject.DTOs.Request;
using MouseTagProject.DTOs.Response;
using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface ICandidate
    {
        Task<PageDto<CandidateResponseDto>> FilterByWhenWasContactedDate(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> FilterByStatus(int statusId, int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> FilterByTechnologies(int[] technologiesId, int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> FilterByNameAndSurname(string candidateNameAndSurname, int pageNumber, int pageSize);
        Task<List<Candidate>> ImportCandidates(IFormFile file);
        Task<PageDto<CandidateResponseDto>> GetCandidates();
        Task<PageDto<CandidateResponseDto>> GetCandidates(int pageNumber, int pageSize);
        Candidate GetCandidate(int id);
        Candidate GetCandidate(string email);
        List<Candidate> GetCandidatesReminder();
        Task<Candidate> AddCandidate(CandidateRequestDto candidate);
        Task UploadCandidateFile(int id, IFormFile file);
        Task FileUpdate(string Url, int id, IFormFile file);
        //Task InitialCandidateFile(string fileURL, IFormFile file);
        Task UpdateCandidate(int id, CandidateRequestDto candidateRequestDto);
        void RemoveCandidate(int id);
        void DeleteCandidates(int[] candidatesIds);
        Task<bool> CheckLinkedIn(string link);
        Task<PageDto<CandidateResponseDto>> GetCandidatesSorted(int pageNumber, int pageSize, List<Candidate> candidates);
        Task<PageDto<CandidateResponseDto>> returnCandidateListNameDesending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListNameAsending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListSurNameDesending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListSurNameAsending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListWhenContactAsending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListWhenContactDesending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListWillContactAsending(int pageNumber, int pageSize);
        Task<PageDto<CandidateResponseDto>> returnCandidateListWillContactDesending(int pageNumber, int pageSize);
    }
}
