namespace MouseTagProject.DTOs.Request
{
    public class CandidateRequestDto
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
        public List<int>? TechnologyIds { get; set; } = new List<int>();
        public List<int>? ClientsIds { get; set; } = new List<int>();
        public string? WhenWasContacted { get; set; }
        public IFormFile? file { get; set; }
    }
}









