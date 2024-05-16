namespace MouseTagProject.DTOs
{
    public class FileUploadDto
    {
        public string? Id { get; set; }
        public string? CVurl { get; set; }
        public IFormFile? file { get; set; }
    }
}
