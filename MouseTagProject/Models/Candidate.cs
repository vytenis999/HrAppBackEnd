namespace MouseTagProject.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Linkedin { get; set; }
        public string? Comment { get; set; }
        public string? OtherStatus { get; set; }
        public string? Salary { get; set; }
        public string? CVurl { get; set; }
        public DateTime? WillBeContacted { get; set; }
        public virtual List<CandidateTechnology>? Technologies { get; set; } = new List<CandidateTechnology>();
        public virtual List<ClientCandidate>? Clients { get; set; } = new List<ClientCandidate>();
        public List<UserDate>? WhenWasContacted { get; set; } = new List<UserDate>();
        public ApplicationUser? Recruiter { get; set; }
        public Status? Status { get; set; }
    }
}
