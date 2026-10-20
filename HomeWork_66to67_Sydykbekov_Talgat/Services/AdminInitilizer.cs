using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class AdminInitilizer
{
    public static async Task SeedAdminUser(RoleManager<IdentityRole<int>> roleManager, 
        UserManager<MyUser> userManager)
    {
        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin123!";
        var roles = new[] {"Admin", "User" , "Employer"};
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                MyUser admin = new MyUser { UserName = adminEmail, Email = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}