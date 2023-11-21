using Persol_HMS.Data.Enums;
using System.Globalization;

namespace Persol_HMS.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
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
                            DepartmentCode = 002
                        },
                        new Department()
                        {
                            DepartmentCode = 003,
                            DepartmentName = "Doctor"
                        },
                        new Department()
                        {
                            DepartmentCode = 001,
                            DepartmentName = "Records"
                        },
                        new Department()
                        {
                            DepartmentCode = 004,
                            DepartmentName = "Lab"
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
                            Dosage = "2x daily",
                            PatientNo = "HMS-1121-2023-K001"
                        }
                    });
                    context.SaveChanges();
                }
                //Lab
                if (!context.Labs.Any())
                {
                    context.Labs.AddRange(new List<Lab>()
                    { new Lab()
                        {
                            Date = DateTime.Now,
                            PatientNo = "HMS-1121-2023-K001",
                            LabName = "Malaria",
                            Result = true,
                            Notes = ""
                        }
                    });
                    context.SaveChanges();
                }
                //Medical
                if (!context.Medicals.Any())
                {
                    context.Medicals.AddRange(new List<Medical>()
                    {
                        new Medical()
                        {
                            Date = DateTime.Now,
                            DateAdmitted = DateTime.Now,
                            VitalsID = 1,
                            Diagnoses = "Has Malaria",
                            DrugsID = 1,
                            LabID = 1,
                            IsAdmitted = true,
                            SymptomsID = 1,
                            WardNo = 1,
                            PatientNo = "HMS-1121-2023-K001"
                        }
                    });
                    context.SaveChanges();
                }
                //Patient
                if (!context.Patients.Any())
                {
                    context.Patients.AddRange(new List<Patient>()
                    {
                        new Patient()
                        {
                            ContactNo = "0223344567",
                            DateOfBirth = new DateTime(2002,7,7),
                            FirstName = "Manny",
                            LastName = "Brown",
                            Gender = 'M',
                            EmergencyContactFirstName = "Tobi",
                            EmergencyContactLastName = "Brown",
                            InsuranceNo = null,
                            InsuranceType = null,
                            EmergencyContactNo = "0553322195"
                        }
                    });
                    context.SaveChanges();
                }
                //Queue
                if (!context.Queues.Any())
                {
                    context.Queues.AddRange(new List<Queue>()
                    {
                        new Queue()
                        {
                            DateToday = DateTime.Now,
                            PatientNo = "HMS-1121-2023-K001",
                            QueueNo = 1,
                            Status = "In progess"
                        }
                    });
                }
                //Symptom
                if (!context.Symptoms.Any())
                {
                    context.Symptoms.AddRange(new List<Symptom>()
                    { 
                        new Symptom()
                        {
                            Date = DateTime.Now,
                            PatientNo = "HMS-1121-2023-K001",
                            Symptoms = "Fever.Headache"
                        }
                    });
                    context.SaveChanges();
                }
                //User
                if (!context.Users.Any())
                {
                    

                }
                //Vital
                if (!context.Vitals.Any())
                {
                    context.Vitals.AddRange(new List<Vital>()
                    {
                        new Vital()
                        {
                            BloodPressure = 123,
                            Date = DateTime.Now,
                            Height = 20,
                            Weight = 20,
                            PatientNo = "HMS-1121-2023-K001",
                            Temperature = 60
                        }
                    });
                }
            }
        }
    }
}
