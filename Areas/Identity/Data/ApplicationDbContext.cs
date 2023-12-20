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

        builder.Entity<DrugStore>().HasData(
            new DrugStore()
            {
                Id = 1,
                DrugName = "Aspirin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 2,
                DrugName = "Lisinopril",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 3,
                DrugName = "Metformin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 4,
                DrugName = "Simvastatin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 5,
                DrugName = "Levothyroxine",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 6,
                DrugName = "Amoxicillin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 7,
                DrugName = "Omeprazole",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 8,
                DrugName = "Atorvastatin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 9,
                DrugName = "Hydrochlorothiazide",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 10,
                DrugName = "Metoprolol",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 11,
                DrugName = "Gabapentin",
                Type = 0 // 0 represents Capsule
            },
            new DrugStore()
            {
                Id = 12,
                DrugName = "Losartan",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 13,
                DrugName = "Amlodipine",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 14,
                DrugName = "Albuterol",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 15,
                DrugName = "Sertraline",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 16,
                DrugName = "Furosemide",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 17,
                DrugName = "Escitalopram",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 18,
                DrugName = "Ibuprofen",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 19,
                DrugName = "Ciprofloxacin",
                Type = 1 // 1 represents Tablet
            },
            new DrugStore()
            {
                Id = 20,
                DrugName = "Codeine",
                Type = 2 // 2 represents Syrup
            },
            new DrugStore()
            {
                Id = 21,
                DrugName = "Promethazine",
                Type = 2 // 2 represents Syrup
            },
            new DrugStore()
            {
                Id = 22,
                DrugName = "Dextromethorphan",
                Type = 2 // 2 represents Syrup
            },
            new DrugStore()
            {
                Id = 23,
                DrugName = "Cough Syrup",
                Type = 2 // 2 represents Syrup
            },
            new DrugStore()
            {
                Id = 24,
                DrugName = "Paracetamol",
                Type = 1 // 1 represents Tablet
            }
        );
    }
    public DbSet<Lab> Labs { get; set; }
    public DbSet<Medical> Medicals { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Vital> Vitals { get; set; }
    public DbSet<User> Staff { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<AdmittedPatient> AdmittedPatients { get; set; }
    public DbSet<Drug> Drugs { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<DailyData> DailyDatas { get; set; }
    public DbSet<DrugStore> DrugStores { get; set; }
}
