using Microsoft.AspNetCore.Identity;    
using Persol_Hms.Models;
using System;

namespace Persol_Hms.Data
{
    public class HMSData
    {
        public static void Seed(IApplicationBuilder applicationBuilder) 
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                //Department
                if (!context.Departments.Any())
                {
                    context.Departments.AddRange(new List<Department>()
                    {
                        new Department()
                        {
                            DepartmentName = "Nursing",
                            DepartmentCode = 001
                        },
                        new Department()
                        {
                            DepartmentCode = 002,
                            DepartmentName = "Doctor"
                        },
                        new Department()
                        {
                            DepartmentName = "Records"
                        }
                    });
                    context.SaveChanges();
                }
                //Drug
                if (!context.Drugs.Any())
                {
                    context.Drugs.AddRange(new List<Drug>()
                    {
                        new Drug()
                        {
                            Date = DateTime.Now,
                            DrugName = "Paracetamol",
                            Dosage = ""
                        }
                    });
                }
                //Lab
                if (!context.Labs.Any())
                {

                }
                //Medical
                if (!context.Medicals.Any())
                {

                }
                //Patient
                if (!context.Patients.Any())
                {

                }
                //Queue
                if (!context.Queues.Any())
                {

                }
                //Symptom
                if (!context.Symptoms.Any())
                {

                }
                //User
                if (!context.Users.Any())
                {

                }
                //Vital
                if (!context.Vitals.Any())
                {

                }
            }
        }
    }
}
