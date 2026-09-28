using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Data
{
    // Put this file in /Data/SeedData.cs
    public static class SeedData
    {
        public static readonly string[] Roles = { "Admin", "Teacher", "Student" };

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Applies pending migrations (creates the database on first run)
            await context.Database.MigrateAsync();

            // 1. Roles
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Default admin
            const string adminEmail = "admin@attendance.com";
            const string adminPassword = "Admin@123";   // change after first login

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 3. Sample department so the dropdowns are not empty
            if (!await context.Departments.AnyAsync())
            {
                context.Departments.Add(new Department { Name = "Computer Engineering" });
                await context.SaveChangesAsync();
            }
        }
    }
}