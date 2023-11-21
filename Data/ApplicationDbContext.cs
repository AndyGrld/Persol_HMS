using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persol_Hms.Models;

namespace Persol_HMS.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
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
