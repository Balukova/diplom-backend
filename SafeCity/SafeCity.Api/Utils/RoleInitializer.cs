using Microsoft.AspNetCore.Identity;
using SafeCity.Api.Entity;

namespace SafeCity.Api.Utils
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            if (await roleManager.FindByNameAsync(UserRoles.Client) == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Client));
            }
            if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin));
            }
        }
    }
}
