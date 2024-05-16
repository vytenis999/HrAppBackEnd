using System.Text.Json.Serialization;

namespace MouseTagProject.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Normalized { get; set; }
        public string Name { get; set; }
        public string? Project { get; set; }
        public string? Comment { get; set; }
        public DateTime? WillBeContacted { get; set; }
        [JsonIgnore]
        public List<ClientCandidate>? Candidates { get; set; }
    }
}
