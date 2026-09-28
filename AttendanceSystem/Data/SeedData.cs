using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Data
{
    public static class SeedData
    {
        public static readonly string[] Roles =
        {
            "Admin",
            "Teacher",
            "Student"
        };

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // =========================
            // 1. Create Roles
            // =========================

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // =========================
            // 2. Create Admin
            // =========================

            await CreateUser(
                userManager,
                "admin@attendance.com",
                "Admin@123",
                "System Admin",
                "Admin"
            );

            // =========================
            // 3. Create Teacher
            // =========================

            await CreateUser(
                userManager,
                "teacher@attendance.com",
                "Teacher@123",
                "Sample Teacher",
                "Teacher"
            );

            // =========================
            // 4. Create Student
            // =========================

            await CreateUser(
                userManager,
                "student@attendance.com",
                "Student@123",
                "Sample Student",
                "Student"
            );

            // =========================
            // 5. Sample Department
            // =========================

            if (!await context.Departments.AnyAsync())
            {
                context.Departments.Add(
                    new Department
                    {
                        Name = "Computer Engineering"
                    }
                );

                await context.SaveChangesAsync();
            }

            // =========================================
            // 6. STUDENT PROFILE - ADDED FOR DASHBOARD
            // =========================================
            // Creates the Student profile linked to the
            // existing student login account.

            var studentUser = await userManager.FindByEmailAsync(
                "student@attendance.com"
            );

            var department = await context.Departments.FirstAsync();

            if (studentUser != null &&
                !await context.Students.AnyAsync(s => s.UserId == studentUser.Id))
            {
                context.Students.Add(
                    new Student
                    {
                        UserId = studentUser.Id,
                        EnrollmentNo = "CE001",
                        Semester = 4,
                        DepartmentId = department.Id
                    }
                );

                await context.SaveChangesAsync();
            }
            // =========================================
            // 7. TEACHER PROFILE - ADDED FOR DASHBOARD
            // =========================================

            var teacherUser = await userManager.FindByEmailAsync(
                "teacher@attendance.com"
            );

            var teacherDepartment = await context.Departments.FirstAsync();

            if (teacherUser != null &&
                !await context.Teachers.AnyAsync(t => t.UserId == teacherUser.Id))
            {
                context.Teachers.Add(
                    new Teacher
                    {
                        UserId = teacherUser.Id,
                        DepartmentId = teacherDepartment.Id
                    }
                );

                await context.SaveChangesAsync();
            }
        }

        // Helper method to create users and assign roles
        private static async Task CreateUser(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create user {email}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}