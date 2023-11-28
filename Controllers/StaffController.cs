using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persol_HMS.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Persol_HMS.Models;
using Persol_HMS.Views.Staff;

// [Authorize]
public class StaffController : Controller
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsUserAuthorized(int departmentId)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
        return user?.DepartmentId == departmentId;
    }

    private IActionResult RedirectToHome()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Doctor(string? patientNo)
    {
        //if (!IsUserAuthorized(3))
        //{
        //    return RedirectToHome();
        //}

        var patientDetails = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(patientNo));

        if (patientDetails != null)
        {
            var medicalViewModel = new CreateMedicalViewModel
            {
                PatientNo = patientNo,
            };
            ViewBag.Name = patientDetails?.FirstName;
            return View(medicalViewModel);
        }

        var nextPatientInLine = GetNextPatientInLine("Doctor");
        if (nextPatientInLine != null)
        {
            // Remove the 'var' keyword here
            patientDetails = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == nextPatientInLine.PatientNo);

            if (patientDetails != null)
            {
                var medicalViewModel = new CreateMedicalViewModel
                {
                    PatientNo = patientDetails.PatientNo,
                };
                ViewBag.Name = nextPatientInLine?.FirstName;
                var doctorQueue = Queue.GetOrCreateQueue(_context, patientDetails.PatientNo, DepartmentType.Doctor);
                return View(medicalViewModel);
            }
        }

        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpPost]
    public async Task<IActionResult> SaveMedicalRecords(DoctorQueueModel model)
    {
        // if (!IsUserAuthorized(3))
        // {
        //     return RedirectToHome();
        // }

        Console.WriteLine(model.CreateMedicalViewModel.PatientNo);
        Console.WriteLine(model.CreateMedicalViewModel.Diagnoses);
        Console.WriteLine(model.CreateMedicalViewModel.DrugName);
        Console.WriteLine(model.CreateMedicalViewModel.Symptoms);
        Console.WriteLine(model.CreateMedicalViewModel.IsAdmitted);
        Console.WriteLine(model.CreateMedicalViewModel.Dosage);
        Console.ReadLine();
        if (!string.IsNullOrEmpty(model.CreateMedicalViewModel.PatientNo) && model.CreateMedicalViewModel.Diagnoses != null && model.CreateMedicalViewModel.Dosage != null &&
            model.CreateMedicalViewModel.DrugName != null && model.CreateMedicalViewModel.Symptoms != null)
        {
            var medicalRecord = new Medical
            {
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                Date = DateTime.Now.Date,
                Diagnoses = model.CreateMedicalViewModel.Diagnoses,
                WardNo = model.CreateMedicalViewModel.IsAdmitted == true ? GenerateWardNumber() : null,                
                IsAdmitted = model.CreateMedicalViewModel.IsAdmitted,
                DateAdmitted = model.CreateMedicalViewModel.IsAdmitted == true ? DateTime.Now.Date : (DateTime?)null
            };

            var drug = new Drug
            {
                ID = _context.Drugs.Count() == 0 ? 1 : _context.Drugs.Max(d => d.ID) + 1,
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                DrugName = model.CreateMedicalViewModel.DrugName,
                Dosage = model.CreateMedicalViewModel.Dosage,
                Date = DateTime.Now.Date
            };
            _context.Drugs.Add(drug);

            var symptom = new Symptom
            {
                ID = _context.Symptoms.Count() == 0 ? 1 : _context.Symptoms.Max(s => s.ID) + 1,
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                Symptoms = model.CreateMedicalViewModel.Symptoms,
                Date = DateTime.Now.Date
            };
            _context.Symptoms.Add(symptom);

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.CreateMedicalViewModel.PatientNo));
            var vital = await _context.Vitals.FirstOrDefaultAsync(v => v.PatientNo.Equals(model.CreateMedicalViewModel.PatientNo));

            if (drug != null)
            {
                medicalRecord.DrugsID = drug.ID;
                medicalRecord.Drug = drug;
            }

            if (symptom != null)
            {
                medicalRecord.SymptomsID = symptom.ID;
                medicalRecord.Symptom = symptom;
            }

            if (patient != null)
            {
                medicalRecord.Patient = patient;
            }

            if (vital != null)
            {
                medicalRecord.Vital = vital;
                medicalRecord.VitalsID = vital.Id;
            }

            _context.Medicals.Add(medicalRecord);
            await _context.SaveChangesAsync();

            var labQueueNo = GetNextQueueNumber("Lab");
            var labQueue = new Queue
            {
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                QueueNo = labQueueNo,
                Status = "Lab",
                DateCreated = DateTime.Now
            };
            RemovePatientFromQueue("Doctor", model.CreateMedicalViewModel.PatientNo);
            _context.Queues.Add(labQueue);
            await _context.SaveChangesAsync();

            TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully. Patient's queue number is {labQueueNo} in the lab queue.";
            return RedirectToAction(nameof(DoctorQueue));
        }

        TempData["D_WarningMessage"] = $"Error processing patient's medical details. Please try again";
        return RedirectToAction(nameof(DoctorQueue));
    }

    private int GenerateWardNumber()
    {
        var maxWardNo = _context.Medicals.Max(m => (int?)m.WardNo);
        int num = maxWardNo.HasValue ? maxWardNo.Value + 1 : 1;
        return num;
    }


    [HttpGet]
    public async Task<IActionResult> RecordsClerk()
    {
        // if (!IsUserAuthorized(1))
        // {
        //     return RedirectToHome();
        // }
        return View(new Patient());
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrGetPatient([Bind("PatientNo, FirstName, LastName, DateOfBirth, ContactNo, InsuranceType, InsuranceNo, Gender, EmergencyContactFirstName, EmergencyContactLastName, EmergencyContactNo")] Patient newPatient)
    {
        // if (!IsUserAuthorized(1))
        // {
        //     return RedirectToHome();
        // }

        if (newPatient.PatientNo != null)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == newPatient.PatientNo);
            if (patient != null)
            {
                var NurseQueueNo = GetNextQueueNumber("Nurse");
                var NurseQueue = new Queue
                {
                    PatientNo = newPatient.PatientNo,
                    QueueNo = NurseQueueNo,
                    Status = "Nurse",
                    DateCreated = DateTime.Now
                };
                _context.Queues.Add(NurseQueue);

                await _context.SaveChangesAsync();

                TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {NurseQueueNo}";
                return RedirectToAction(nameof(RecordsClerk));
            }
            TempData["R_WarningMessage"] = $"{newPatient.PatientNo} - Patient not found";
            return RedirectToAction(nameof(RecordsClerk));
        }

        newPatient.PatientNo = GenerateNewId(newPatient);
        newPatient.Id = _context.Patients.Count() == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;

        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();

        var nurseQueueNo = GetNextQueueNumber("Nurse");
        var nurseQueue = new Queue
        {
            PatientNo = newPatient.PatientNo,
            QueueNo = nurseQueueNo,
            Status = "Nurse",
            DateCreated = DateTime.Now
        };
        _context.Queues.Add(nurseQueue);

        await _context.SaveChangesAsync();

        TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {nurseQueueNo}";
        return RedirectToAction(nameof(RecordsClerk));
    }

    public string GenerateNewId(Patient patient)
    {
        DateTime currentDate = DateTime.Now;
        char[] name = patient.LastName.ToCharArray();
        string idPrefix = $"HMS-{currentDate.Month}{currentDate.Day}-{currentDate.Year}-" +
            $"{name[0].ToString().ToUpper()}";
        string id;
        int newId = 1;
        do
        {
            id = $"{idPrefix}{newId:D3}";
            newId++;
        } while (_context.Patients.Any(p => p.PatientNo == id));

        return id;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPatient(string patientNo)
    {
        if (!string.IsNullOrEmpty(patientNo))
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == patientNo);
            if (patient != null)
            {
                return View("RecordsClerk", patient);
            }
        }

        return RedirectToAction(nameof(RecordsClerk));
    }

    [HttpGet]
    // [Authorize(Roles = "Nursing")]
    public IActionResult Nurse(string? patientNo)
    {
        // if (!IsUserAuthorized(2))
        // {
        //     return RedirectToHome();
        // }
        if (patientNo != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
            var vitalModel = new Vital
            {
                PatientNo = patientDetails?.PatientNo
            };
            if (!string.IsNullOrEmpty(vitalModel.PatientNo))
            {
                var nurseQueue = Queue.GetOrCreateQueue(_context, vitalModel.PatientNo, DepartmentType.Nurse);
            }
            ViewBag.Name = patientDetails?.FirstName;
            return View(vitalModel);
        }

        var nextPatientInLine = GetNextPatientInLine("Nurse");
        if (nextPatientInLine != null)
        {
            var vitalModel = new Vital
            {
                PatientNo = nextPatientInLine?.PatientNo
            };
            ViewBag.Name = nextPatientInLine?.FirstName;
            if (!string.IsNullOrEmpty(vitalModel.PatientNo))
            {
                var nurseQueue = Queue.GetOrCreateQueue(_context, vitalModel.PatientNo, DepartmentType.Nurse);
            }

            
            return View(vitalModel);
        }

        return RedirectToAction(nameof(NurseQueue));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nurse([Bind("PatientNo, Temperature, Height, Weight, BloodPressure")] Vital vital)
    {
        // if (!IsUserAuthorized(2))
        // {
        //     return RedirectToHome();
        // }
        if (!string.IsNullOrEmpty(vital.PatientNo) &&
            vital.Temperature != null &&
            vital.Height != null &&
            vital.Weight != null &&
            vital.BloodPressure != null)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == vital.PatientNo);
            if (patient != null)
            {
                _context.Vitals.Update(vital);

                var doctorQueueNo = GetNextQueueNumber("Doctor");
                var doctorQueue = new Queue
                {
                    PatientNo = vital.PatientNo,
                    QueueNo = doctorQueueNo,
                    Status = "Doctor",
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Nurse", patient.PatientNo);
                _context.Queues.Add(doctorQueue);

                await _context.SaveChangesAsync();
                TempData["N_ConfirmationMessage"] = $"Patient's vitals added successfully. Patient's queue number is {doctorQueueNo} in the doctor queue.";
                return RedirectToAction(nameof(NurseQueue));
            }
        }
        TempData["N_WarningMessage"] = $"Error processing patient's vitals. Please try again";
        return RedirectToAction(nameof(NurseQueue));
    }
	
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lab([Bind("PatientNo, LabName, Result, Notes")] Persol_HMS.Models.Lab lab)
    {
        if (!string.IsNullOrEmpty(lab.PatientNo) &&
            lab.LabName != null &&
            lab.Result != null &&
            lab.Notes != null)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == lab.PatientNo);
            
            if (patient != null)
            {

                var labEntry = new Persol_HMS.Models.Lab
                {
                    ID = _context.Labs.Count() == 0 ? 1 : _context.Labs.Max(s => s.ID) + 1,
                    PatientNo = lab.PatientNo,
                    LabName = lab.LabName,
                    Result = lab.Result,
                    Notes = lab.Notes,
                    Date = lab.Date,
                };
                RemovePatientFromQueue("Lab", patient.PatientNo);
                _context.Labs.Add(labEntry);
                await _context.SaveChangesAsync();
                TempData["ConfirmationMessage"] = $"Patient's lab added successfully";
                return RedirectToAction(nameof(Lab));

            }

        }

        TempData["WarningMessage"] = "Error processing patient's lab. Please try again";
        return RedirectToAction(nameof(LabQueue));
    }


	
    public IActionResult NurseQueue(int page = 1, string search = "")
    {
        // if (!IsUserAuthorized(2))
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Nurse" &&
                        (q.PatientNo.Contains(search) ||
                        q.Patient.FirstName.Contains(search) ||
                        q.Patient.LastName.Contains(search)))
            .OrderBy(q => q.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var patientsInLine = query.ToList();
        var totalPatients = query.Count();

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };

        var viewModel = new VitalsQueueModel
        {
            Vital = new Vital(),
            QueueViewModel = model
        };

        return View(viewModel);
    }
	
	
    public IActionResult LabQueue(int page = 1, string search = "")
    {
        // if (!IsUserAuthorized(4))
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Lab" &&
                        (q.PatientNo.Contains(search) ||
                        q.Patient.FirstName.Contains(search) ||
                        q.Patient.LastName.Contains(search)))
            .OrderBy(q => q.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var patientsInLine = query.ToList();
        var totalPatients = query.Count();

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };
		
		var viewModel = new LabQueueViewModel
        {
            Lab = new Persol_HMS.Models.Lab(),
            QueueViewModel = model
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Lab(string? patientNo)
    {
        // if (!IsUserAuthorized(4))
        // {
        //     return RedirectToHome();
        // }
        if (patientNo != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
            var labEntry = new Persol_HMS.Models.Lab
            {
                PatientNo = patientDetails.PatientNo
            };
            if (!string.IsNullOrEmpty(labEntry.PatientNo))
            {
                var labQueue = Queue.GetOrCreateQueue(_context, labEntry.PatientNo, DepartmentType.Lab);
            }
            ViewBag.Name = patientDetails.FirstName + " " + patientDetails.LastName;
            return View(labEntry);
        }

        var patientsInQueue = GetNextPatientInLine("Lab");
        if (patientsInQueue != null)
        {
            var labEntry = new Persol_HMS.Models.Lab
            {
                PatientNo = patientsInQueue.PatientNo
            };
            if (!string.IsNullOrEmpty(labEntry.PatientNo))
            {
                var LabQueue = Queue.GetOrCreateQueue(_context, labEntry.PatientNo, DepartmentType.Lab);
            }
            ViewBag.Name = patientsInQueue.FirstName + " " + patientsInQueue.LastName;
            return View(labEntry);
        }

        return RedirectToAction(nameof(LabQueue));
    }
	
    public IActionResult DoctorQueue(int page = 1, string search = "")
    {
        // if (!IsUserAuthorized(3))
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Doctor" &&
                        (q.PatientNo.Contains(search) ||
                        q.Patient.FirstName.Contains(search) ||
                        q.Patient.LastName.Contains(search)))
            .OrderBy(q => q.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var patientsInLine = query.ToList();
        var totalPatients = query.Count();

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };

        var viewModel = new DoctorQueueModel
        {
            CreateMedicalViewModel = new CreateMedicalViewModel(),
            QueueViewModel = model
        };

        return View(viewModel);
    }

    private Patient GetNextPatientInLine(string status)
    {
        var nextPatientInLine = _context.Queues
            .Where(q => q.Status == status && q.DateCreated.Date == DateTime.Now.Date)
            .OrderBy(q => q.QueueNo)
            .FirstOrDefault();

        if (nextPatientInLine != null)
        {
            var nextPatient = _context.Patients.FirstOrDefault(p => p.PatientNo == nextPatientInLine.PatientNo);
            return nextPatient;
        }

        return null;
    }

    private void RemovePatientFromQueue(string status, string patientNo)
    {
        var patientQueue = _context.Queues.FirstOrDefault(q => q.Status == status && q.PatientNo == patientNo);

        if (patientQueue != null)
        {
            _context.Queues.Remove(patientQueue);
            _context.SaveChanges();
        }
    }

    private int GetNextQueueNumber(string status)
    {
        var maxQueueNumber = _context.Queues
            .Where(q => q.Status == status && q.DateCreated.Date == DateTime.Now.Date)
            .Max(q => (int?)q.QueueNo) ?? 0;
        return maxQueueNumber + 1;
    }

	
    public IActionResult PatientList(int page = 1, string search = "")
    {
        int pageSize = 10;
        var patients = _context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            patients = patients.Where(p =>
                                p.PatientNo.Contains(search) ||
                                p.FirstName.Contains(search) ||
                                p.LastName.Contains(search))
                                .OrderBy(q => q.PatientNo)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize);
        }
        ViewBag.TotalPatients = _context.Patients.Count();
        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = page;
        ViewBag.Search = search;

        return View(patients.ToList());
    }

	
    public IActionResult PatientMedicalRecords(string patientNo)
    {
        var patient = _context.Patients
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Vital)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Symptom)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Drug)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Lab)
            .FirstOrDefault(p => p.PatientNo == patientNo);

        if (patient == null)
        {
            return NotFound();
        }

        var medicalViewModels = patient.Medicals.Select(m => new MedicalViewModel
        {
            ID = m.ID,
            Date = m.Date,
            Vital = m.Vital,
            Diagnoses = m.Diagnoses,
            Symptom = m.Symptom,
            Drug = m.Drug,
            Lab = m.Lab
        }).ToList();

        var viewModel = new PatientMedicalViewModel
        {
            Patient = patient,
            MedicalRecords = medicalViewModels
        };

        return View(viewModel);
    }

}
