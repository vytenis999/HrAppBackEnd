using System.Text.Json.Serialization;

namespace MouseTagProject.Models
{
    public class Technology
    {
        public int Id { get; set; }
        public string TechnologyName { get; set; }
        [JsonIgnore]
        public virtual List<CandidateTechnology> Candidates { get; set; }
    }
}
