using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persol_HMS.Models;

namespace Persol_HMS.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
    public DbSet<Lab> Labs { get; set; }
    public DbSet<Medical> Medicals { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Symptom> Symptoms { get; set; }
    public DbSet<Vital> Vitals { get; set; }
    public DbSet<User> Staff { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Drug> Drugs { get; set; }
    public DbSet<Department> Departments { get; set; }
}
