namespace MouseTagProject.DTOs.Response
{
    public class CandidateResponseDto
    {
        public int Id { get; set; }
        public List<string>? WhenWasContacted { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Linkedin { get; set; }
        public string? Comment { get; set; }
        public int? StatusId { get; set; }
        public string? CVurl { get; set; }
        public string? Salary { get; set; }
        public string? Recruiter { get; set; }
        public List<TechnologyDto>? Technologies { get; set; }
        public List<ClientResponseDto>? Clients { get; set; }
        public string? WillBeContacted { get; set; }
        public string? OtherStatus { get; set; }

    }
}
