using kfh_expense_app.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace kfh_expense_app.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {

        string[] roles = new[] { "Employee", "Approver", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        //Seed users
        var users = new[]
        {
            new { Name = "employee1", Pass = "Hello123", Role = "Employee" },
            new{ Name = "approver1", Pass = "Hello123", Role = "Approver" },
            new{ Name = "admin1", Pass ="Hello123", Role = "Admin" }
        };

        foreach (var item in users)
        {
            if (await userManager.FindByNameAsync(item.Name) is not null)
            {
                continue;
            }

            var user = new AppUser
            {
                UserName = item.Name,
                Role = item.Role,
                Email = $"{item.Name}@expense.com",
            };

            await userManager.CreateAsync(user, item.Pass);
            await userManager.AddToRoleAsync(user, item.Role);

        }
    }
}
