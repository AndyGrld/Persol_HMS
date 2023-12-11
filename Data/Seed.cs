using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure the database is created.
        context.Database.EnsureCreated();

        // Check if users already exist
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        // Seed users
        var users = new List<UserSeedData>
        {
            new UserSeedData
            {
                UserName = "Records_01",
                Email = "Records_01@example.com",
                FirstName = "John",
                MiddleName = "L.",
                LastName = "Doe",
                DepartmentId = 001,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Records_01"
            },
            new UserSeedData
            {
                UserName = "Nurse_01",
                Email = "Nurse_01@example.com",
                FirstName = "Emma",
                MiddleName = "Rose",
                LastName = "Brown",
                DepartmentId = 002,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Nurse_01"
            },
            new UserSeedData
            {
                UserName = "Doctor_01",
                Email = "Doctor_01@example.com",
                FirstName = "Liam",
                MiddleName = "Alexander",
                LastName = "Williams",
                DepartmentId = 003,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Doctor_01",
            },
            new UserSeedData
            {
                UserName = "Lab_01",
                Email = "Lab_01@example.com",
                FirstName = "Noah  ",
                MiddleName = "Benjamin",
                LastName = "Jones",
                DepartmentId = 004,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Lab_01"
            },
            new UserSeedData
            {
                UserName = "Admin",
                Email = "admin@example.com",
                FirstName = "Ethan",
                MiddleName = "James",
                LastName = "Smith",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                DepartmentId = 005,
                Password = "Admin_01"
            },
            new UserSeedData
            {
                UserName = "Pharmacy_01",
                Email = "Pharmacy@example.com",
                FirstName = "Ava",
                MiddleName = "Marie",
                LastName = "Garcia",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                DepartmentId = 006,
                Password = "Pharmacy_01"
            },
            new UserSeedData
            {
                UserName = "Cashier_01",
                Email = "cashier@example.com",
                FirstName = "Sophia",
                MiddleName = "Elizabeth",
                LastName = "Davis",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                DepartmentId = 007,
                Password = "Cashier_01"
            }

            // Add more users as needed
        };

        foreach (var userSeedData in users)
        {
            var user = new User
            {
                UserName = userSeedData.UserName,
                Email = userSeedData.Email,
                FirstName = userSeedData.FirstName,
                MiddleName = userSeedData.MiddleName,
                LastName = userSeedData.LastName,
                DepartmentId = userSeedData.DepartmentId,
                DateOfBirth = userSeedData.DateOfBirth,
                Status = "Active",
                CreatedDate = DateTime.Now,
                Attempts = 0,
                LockEnabled = false,
            };

            var result = await userManager.CreateAsync(user, userSeedData.Password);

            if (result.Succeeded)
            {
                // Assign roles to users if needed
                // Example: await userManager.AddToRoleAsync(user, "UserRole");
            }
            else
            {
                // Handle errors if user creation fails
                throw new Exception($"Failed to create user {userSeedData.UserName}: {string.Join(", ", result.Errors)}");
            }
        }

        // Save changes to the database
        await context.SaveChangesAsync();
    }

    private class UserSeedData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
    }
}
