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

                if (!context.Users.Any())
                {
                    context.Users.AddRange(new List<User>
                    {
                        new User()
                        {
                            AccessFailedCount = 0,
                            Attempts = 0,
                            CreatedDate = DateTime.Now,
                            DateOfBirth = new DateTime(2000,06,17),
                            LockEnabled = false,
                            EmailConfirmed = false,
                            Email = "James@Gmail.com",
                            DepartmentId = 001,
                            FirstName = "James",
                            LockEnd = DateTime.Now,
                            MiddleName = null,
                            LastName = "John",
                            LockoutEnd = DateTime.Now,
                            PasswordHash = "James123",
                            Status = "Active",
                            LockoutEnabled = false,
                            NormalizedEmail = "james@hospital.com",
                            PhoneNumberConfirmed = false,
                            NormalizedUserName = null,
                            PhoneNumber = null,
                            SecurityStamp = null,
                            ConcurrencyStamp = null,
                            TwoFactorEnabled = false,
                            UserName = "James"
                        },
                        new User()
                        {
                            AccessFailedCount = 0,
                            Attempts = 0,
                            CreatedDate = DateTime.Now,
                            DateOfBirth = new DateTime(2000,06,17),
                            LockEnabled = false,
                            EmailConfirmed = false,
                            Email = "Paul@Gmail.com",
                            DepartmentId = 003,
                            FirstName = "Paul",
                            LockEnd = DateTime.Now,
                            MiddleName = null,
                            LastName = "John",
                            LockoutEnd = DateTime.Now,
                            PasswordHash = "Paul123",
                            Status = "Active",
                            LockoutEnabled = false,
                            NormalizedEmail = "paul@hospital.com",
                            PhoneNumberConfirmed = false,
                            NormalizedUserName = null,
                            PhoneNumber = null,
                            SecurityStamp = null,
                            ConcurrencyStamp = null,
                            TwoFactorEnabled = false,
                            UserName = "Paul"
                        },

                        new User()
                        {
                            AccessFailedCount = 0,
                            Attempts = 0,
                            CreatedDate = DateTime.Now,
                            DateOfBirth = new DateTime(2000,06,17),
                            LockEnabled = false,
                            EmailConfirmed = false,
                            Email = "Ama@Gmail.com",
                            DepartmentId = 002,
                            FirstName = "Ama",
                            LockEnd = DateTime.Now,
                            MiddleName = null,
                            LastName = "John",
                            LockoutEnd = DateTime.Now,
                            PasswordHash = "Ama123",
                            Status = "Active",
                            LockoutEnabled = false,
                            NormalizedEmail = "ama@hospital.com",
                            PhoneNumberConfirmed = false,
                            NormalizedUserName = null,
                            PhoneNumber = null,
                            SecurityStamp = null,
                            ConcurrencyStamp = null,
                            TwoFactorEnabled = false,
                            UserName = "Ama"
                        },

                        new User()
                        {
                            AccessFailedCount = 0,
                            Attempts = 0,
                            CreatedDate = DateTime.Now,
                            DateOfBirth = new DateTime(2000,06,17),
                            LockEnabled = false,
                            EmailConfirmed = false,
                            Email = "Nana@Gmail.com",
                            DepartmentId = 004,
                            FirstName = "Nana",
                            LockEnd = DateTime.Now,
                            MiddleName = null,
                            LastName = "John",
                            LockoutEnd = DateTime.Now,
                            PasswordHash = "Nana123",
                            Status = "Active",
                            LockoutEnabled = false,
                            NormalizedEmail = "nana@hospital.com",
                            PhoneNumberConfirmed = false,
                            NormalizedUserName = null,
                            PhoneNumber = null,
                            SecurityStamp = null,
                            ConcurrencyStamp = null,
                            TwoFactorEnabled = false,
                            UserName = "Nana"
                        }


                    });

                }
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
                            Result = "Very ill",
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
                            DateCreated = DateTime.Now,
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
