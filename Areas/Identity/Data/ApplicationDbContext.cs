using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persol_HMS.Models;

namespace Persol_HMS.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



        builder.Entity<Patient>()
            .HasMany(p => p.Medicals)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientNo);

        builder.Entity<Department>().HasData(
            new Department()
                        {
                            Id = 2,
                            DepartmentName = "Nursing",
                            DepartmentCode = 002
                        },
                        new Department()
                        {
                            Id = 3,
                            DepartmentCode = 003,
                            DepartmentName = "Doctor"
                        },
                        new Department()
                        {
                            Id = 1,
                            DepartmentCode = 001,
                            DepartmentName = "Records"
                        },
                        new Department()
                        {
                            Id = 4,
                            DepartmentCode = 004,
                            DepartmentName = "Lab"
                        },
                        new Department()
                        {
                            Id = 5,
                            DepartmentCode = 005,
                            DepartmentName = "Admin"
                        },
                        new Department()
                        {
                            Id = 6,
                            DepartmentCode = 006,
                            DepartmentName = "Pharmacy"
                        },
                        new Department()
                        {
                            Id = 7,
                            DepartmentCode = 007,
                            DepartmentName = "Cashier"
                        }
        );
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
