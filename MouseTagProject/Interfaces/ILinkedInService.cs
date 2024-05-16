using MouseTagProject.DTOs;

namespace MouseTagProject.Interfaces
{
    public interface ILinkedInService
    {
        NameAndSurnameDto ScrapeSerp(string linkedInLink);
    }
}
