using System.Text.Json.Serialization;

namespace MouseTagProject.Models
{
    public class ClientCandidate
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int CandidateId { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        [JsonIgnore]
        public Candidate Candidate { get; set; }
    }
}
