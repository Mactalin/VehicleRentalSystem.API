using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Default admin (change the password after first login)
            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                context.Users.Add(new User
                {
                    RoleId = adminRole.Id,
                    FullName = "System Admin",
                    Email = "admin@vehiclerental.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                });
            }

            // Categories with base pricing
            if (!await context.VehicleCategories.AnyAsync())
            {
                context.VehicleCategories.AddRange(
                    NewCategory("Car", "Hatchbacks and sedans", 150, 1500, 9000, 5000),
                    NewCategory("SUV", "SUVs and MUVs", 250, 2500, 15000, 8000),
                    NewCategory("Van", "Passenger and cargo vans", 300, 3000, 18000, 10000),
                    NewCategory("Bike", "Motorcycles and scooters", 50, 500, 3000, 2000));
            }

            // Main branch with Mon–Sun hours
            if (!await context.Branches.AnyAsync())
            {
                var branch = new Branch
                {
                    Name = "Main Branch",
                    Address = "Central Bus Stand Road",
                    City = "Tiruchirappalli",
                    State = "Tamil Nadu",
                    Pincode = "620001"
                };

                foreach (var day in Enum.GetValues<DayOfWeek>())
                {
                    branch.OperatingHours.Add(new BranchOperatingHour
                    {
                        DayOfWeek = day,
                        OpenTime = new TimeOnly(8, 0),
                        CloseTime = new TimeOnly(20, 0),
                        IsClosed = false
                    });
                }
                context.Branches.Add(branch);
            }

            await context.SaveChangesAsync();
        }

        private static VehicleCategory NewCategory(
            string name, string description,
            decimal hourly, decimal daily, decimal weekly, decimal deposit) => new()
            {
                Name = name,
                Description = description,
                Pricing = new CategoryPricing
                {
                    HourlyPrice = hourly,
                    DailyPrice = daily,
                    WeeklyPrice = weekly,
                    SecurityDeposit = deposit
                }
            };
    }
}
