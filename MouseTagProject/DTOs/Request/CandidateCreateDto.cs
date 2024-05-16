namespace MouseTagProject.DTOs.Request
{
    public class CandidateCreateDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Linkedin { get; set; }
        public string? Comment { get; set; } = "";
        public int? StatusId { get; set; }
        public string? OtherStatus { get; set; }
        public string? Salary { get; set; }
        public string? CVurl { get; set; }
        public string? WillBeContacted { get; set; }
        public string? TechnologyIds { get; set; } 
        public string? ClientsIds { get; set; } 
        public string? WhenWasContacted { get; set; }
        public IFormFile? file { get; set; }
    }
}
