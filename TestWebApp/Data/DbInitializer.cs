using Microsoft.AspNetCore.Identity;
using TestWebApp.Models;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<AppUser> userManager)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Administrator", "Manager", "Employee" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        string adminEmail = "admin@test.com";
        string adminPassword = "Admin123!";
        string adminFirstName = "Admin";
        string adminLastName = "Admin";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new AppUser 
            { 
                UserName = adminEmail, 
                Email = adminEmail,
                FirstName = adminFirstName,
                LastName = adminLastName,
            };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }
}