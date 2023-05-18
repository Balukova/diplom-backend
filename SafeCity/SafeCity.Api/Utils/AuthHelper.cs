using System.Security.Claims;

namespace SafeCity.Api.Utils
{
    public static class AuthHelper
    {
        public static int GetClUserId(this ClaimsPrincipal principal)
        {
            string userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var userIdInt = Convert.ToInt32(userId);
            return userIdInt;
        }
    }
}
