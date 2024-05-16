using Microsoft.AspNetCore.Identity;
using MouseTagProject.Models;

namespace MouseTagProject.Context.Seeders
{
    public class IdentitySeed
    {
        public static async Task Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            await SeedRoles(roleManager);
            await SeedAdmin(userManager, config);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.Roles.Count() > 0) return;
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        private static async Task SeedAdmin(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            if (userManager.Users.Any(u => u.Id == "664177c7-7db1-421e-a116-6b264103fef5")) return;
            var rootUser = new ApplicationUser {Id = "664177c7-7db1-421e-a116-6b264103fef5", UserName = config["RootUser:UserName"], Email = config["RootUser:UserName"] };
            await userManager.CreateAsync(rootUser, config["RootUser:Password"]);
            await userManager.AddToRoleAsync(rootUser, "SuperAdmin");
        }
    }
}
