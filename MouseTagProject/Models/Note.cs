using System.ComponentModel.DataAnnotations.Schema;

namespace MouseTagProject.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public string userID { get; set; }
        public string color { get; set; }
    }
}
