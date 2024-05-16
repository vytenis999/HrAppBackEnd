namespace MouseTagProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Note> Notes { get; set; }
        public string userID { get; set; }
        public string color { get; set; }
    }
}
