using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;

namespace Project_3.src.Infrastructure.Data.Seed
{
    public class SeedData
    {
        public static async Task Initialize(
            AppDbContext context,
            UserManager<User> userManager)
        {
            // Seed Departments
            if (!await context.Departments.AnyAsync())
            {
                await context.Departments.AddRangeAsync(
                    new Department { Name = "Human Resources" },
                    new Department { Name = "Finance" },
                    new Department { Name = "IT" }
                );

                await context.SaveChangesAsync();
            }

            // Seed Users
            if (await userManager.FindByEmailAsync("john@gmail.com") == null)
            {
                var admin = new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "john@gmail.com",
                    Email = "john@gmail.com",
                    DepartmentId = 1,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Password@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("jane@gmail.com") == null)
            {
                var manager = new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserName = "jane@gmail.com",
                    Email = "jane@gmail.com",
                    DepartmentId = 2,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(manager, "Password@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Manager");
                }
            }

            if (await userManager.FindByEmailAsync("bob@gmail.com") == null)
            {
                var employee = new User
                {
                    FirstName = "Bob",
                    LastName = "Johnson",
                    UserName = "bob@gmail.com",
                    Email = "bob@gmail.com",
                    DepartmentId = 3,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(employee, "Password@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, "Employee");
                }
            }

            // Seed Leave Types
            if (!await context.LeaveTypes.AnyAsync())
            {
                context.LeaveTypes.AddRange(
                    new LeaveType
                    {
                        Name = "Annual",
                        DefaultDays = 21,
                        Description = "Paid time off for vacation or personal reasons."
                    },
                    new LeaveType
                    {
                        Name = "Sick",
                        DefaultDays = 14,
                        Description = "Paid time off for illness or medical appointments."
                    },
                    new LeaveType
                    {
                        Name = "Casual",
                        DefaultDays = 7,
                        Description = "Paid time off for personal or family matters."
                    },
                    new LeaveType
                    {
                        Name = "Unpaid",
                        DefaultDays = 0,
                        Description = "Time off without pay."
                    }
                );
            }

            // Seed Holidays
            if (!await context.Holidays.AnyAsync())
            {
                await context.Holidays.AddRangeAsync(
                    new Holiday
                    {
                        Name = "Revolution Day",
                        Date = new DateOnly(2026, 7, 23),
                        CountryCode = "EG"
                    },
                    new Holiday
                    {
                        Name = "Armed Forces Day",
                        Date = new DateOnly(2026, 10, 6),
                        CountryCode = "EG"
                    }
                );
            }

            await context.SaveChangesAsync();

            if (!await context.EmployeeLeaveBalances.AnyAsync())
            {
                var users = await context.Users.ToListAsync();
                var leaveTypes = await context.LeaveTypes.ToListAsync();

                var balances = new List<EmployeeLeaveBalance>();

                foreach (var user in users)
                {
                    foreach (var leaveType in leaveTypes)
                    {
                        balances.Add(new EmployeeLeaveBalance
                        {
                            UserId = user.Id,
                            LeaveTypeId = leaveType.Id,
                            RemainingDays = leaveType.DefaultDays
                        });
                    }
                }

                await context.EmployeeLeaveBalances.AddRangeAsync(balances);
                await context.SaveChangesAsync();
            }
        }
    }
}