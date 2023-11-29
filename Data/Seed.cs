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
                MiddleName = "l",
                LastName = "Doe",
                DepartmentId = 001,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Records_01"
            },
            new UserSeedData
            {
                UserName = "Nurse@123",
                Email = "Nurse_01@example.com",
                FirstName = "Ama",
                MiddleName = "l",
                LastName = "Sam",
                DepartmentId = 002,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Nurse_01"
            },
            new UserSeedData
            {
                UserName = "Doctor@123",
                Email = "Doctor_01@example.com",
                FirstName = "Doc",
                MiddleName = "d",
                LastName = "James",
                DepartmentId = 003,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Doctor_01",
            },
            new UserSeedData
            {
                UserName = "Lab@123",
                Email = "Lab_01@example.com",
                FirstName = "Lab",
                MiddleName = "l",
                LastName = "Strange",
                DepartmentId = 004,
                DateOfBirth = DateTime.Parse("1990-01-01"),
                Password = "Lab@123"
            },
            new UserSeedData
            {
                UserName = "Admin",
                Email = "admin@example.com",
                FirstName = "Admin",
                MiddleName = "l",
                LastName = "Admin",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                DepartmentId = 005,
                Password = "Admin@123"
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
