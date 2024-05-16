using MouseTagProject.Interfaces;
using System.Security.Claims;

namespace MouseTagProject.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            var currentUserIdClaim = GetUserClaim();

            if (currentUserIdClaim is not null)
            {
                return currentUserIdClaim;
            }

            const string errorMessage = "Invalid token - no user ID.";
            throw new InvalidOperationException(errorMessage);
        }

        private string GetUserClaim()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var userClaims = identity.Claims;
            var id = userClaims.FirstOrDefault(x => x.Type == "Id")?.Value;
            return id;
        }
    }
}
