using Microsoft.AspNetCore.Identity;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Data.Seed;

namespace Project_3.src.API.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedRolesAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Admin", "Employee","Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        // The new method to call from Program.cs
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            // First, run your existing role seeder
            await app.SeedRolesAsync();

            // Next, run the data seeder (Users, Leave Types, Balances)
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await SeedData.Initialize(context, userManager);
        }

    }
}
