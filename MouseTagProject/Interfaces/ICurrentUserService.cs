using System.Security.Claims;

namespace MouseTagProject.Interfaces
{
    public interface ICurrentUserService
    {
        string GetCurrentUserId();
    }
}
