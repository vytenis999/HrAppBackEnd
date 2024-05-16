using System.Text.Json.Serialization;

namespace MouseTagProject.Models
{
    public class CandidateTechnology
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int TechnologyId { get; set; }
        [JsonIgnore]
        public Candidate Candidate { get; set; }
        [JsonIgnore]
        public Technology Technology { get; set; }

    }
}
