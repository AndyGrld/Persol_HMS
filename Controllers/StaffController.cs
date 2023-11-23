using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persol_HMS.Models;
using Persol_HMS.Views.Staff;

public class StaffController : Controller
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    // public IActionResult YourAction()
    // {
    //     if (User.Identity.IsAuthenticated)
    //     {
    //         string username = User.Identity.Name;
    //     }

    //     return View();
    // }

    [HttpGet]
    public IActionResult Doctor(string? patientNo)
    {
        // Automatically fetch the next patient in line for the doctor
        var nextPatientInLine = GetNextPatientInLine("Doctor");

        if (patientNo != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
            if (patientDetails == null)
            {
                var medicalViewModel = new CreateMedicalViewModel
                {
                    PatientNo = patientNo,
                    FirstName = patientDetails.FirstName,
                    LastName = patientDetails.LastName
                };
                return View(medicalViewModel);
            }
        }

        if (nextPatientInLine != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == nextPatientInLine.PatientNo);

            if (patientDetails != null)
            {
                var medicalViewModel = new CreateMedicalViewModel
                {
                    PatientNo = patientNo,
                    FirstName = patientDetails.FirstName,
                    LastName = patientDetails.LastName
                };
                var doctorQueue = Queue.GetOrCreateQueue(_context, patientDetails.PatientNo, DepartmentType.Doctor);
                return View(medicalViewModel);
            }
        }

        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveMedicalRecords(CreateMedicalViewModel model)
    {
        if (ModelState.IsValid)
        {
            var medicalRecord = new Medical
            {
                PatientNo = model.PatientNo,
                Date = DateTime.Now,
                Diagnoses = model.Diagnoses,
                WardNo = GenerateWardNumber(),
                IsAdmitted = model.IsAdmitted,
                DateAdmitted = model.DateAdmitted ?? DateTime.Now
            };

            var drug = new Drug
            {
                PatientNo = model.PatientNo,
                DrugName = model.DrugName,
                Dosage = model.Dosage,
                Date = DateTime.Now
            };
            var symptom = new Symptom
            {
                PatientNo = model.PatientNo,
                Symptoms = model.Symptoms,
                Date = DateTime.Now
            };

            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync();

            _context.Symptoms.Add(symptom);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.PatientNo));
            var vital = await _context.Vitals.FirstOrDefaultAsync(v => v.PatientNo.Equals(model.PatientNo));
            drug = await _context.Drugs.FirstOrDefaultAsync(d => d.PatientNo.Equals(model.PatientNo) && (d.Date.Minute < (DateTime.Now.Minute - 2))); // less than 2 minutes
            symptom = await _context.Symptoms.FirstOrDefaultAsync(s => s.PatientNo.Equals(model.PatientNo) && (s.Date.Minute < (DateTime.Now.Minute - 2)));

            if(drug != null)
            {
                medicalRecord.DrugsID = drug.ID;
                medicalRecord.Drug = drug;
            }
            if(symptom != null)
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

            // remove diagnosed patient from doctor and into lab
            var labQueueNo = GetNextQueueNumber("Lab");
            var labQueue = new Queue
            {
                PatientNo = model.PatientNo,
                QueueNo = labQueueNo,
                Status = "Lab",
                DateToday = DateTime.Now
            };
            RemovePatientFromQueue("Doctor", model.PatientNo);
            _context.Queues.Add(labQueue);
            await _context.SaveChangesAsync();

            TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully. Patient's queue number is {labQueueNo} in the lab queue.";
            return RedirectToAction(nameof(Doctor));
        }

        TempData["D_WarningMessage"] = $"Error processing patient's medical details. Please try again";
        return RedirectToAction(nameof(Doctor));
    }

    private int GenerateWardNumber()
    {
        int num = _context.Medicals.ToList().Count == 0 ? 1 : _context.Medicals.Max(m => m.WardNo) + 1;
        return num;
    }


    [HttpGet]
    public IActionResult RecordsClerk()
    {
        return View(new Patient());
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrGetPatient([Bind("PatientNo, FirstName, LastName, DateOfBirth, ContactNo, InsuranceType, InsuranceNo, Gender, EmergencyContactFirstName, EmergencyContactLastName, EmergencyContactNo")] Patient newPatient)
    {
        if (newPatient.PatientNo != String.Empty)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNo == newPatient.PatientNo);
            if (patient != null)
            {
                // Add the patient to the nurse queue
                var NurseQueueNo = GetNextQueueNumber("Nurse");
                var NurseQueue = new Queue
                {
                    PatientNo = newPatient.PatientNo,
                    QueueNo = NurseQueueNo,
                    Status = "Nurse",
                    DateToday = DateTime.Now
                };
                _context.Queues.Add(NurseQueue);

                await _context.SaveChangesAsync();

                TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {NurseQueueNo}";
                return RedirectToAction(nameof(RecordsClerk));
            }
            TempData["R_WarningMessage"] = $"{newPatient.PatientNo} - Patient not found";
            return RedirectToAction(nameof(RecordsClerk));
        }
        // Generate and set the patient ID
        newPatient.PatientNo = GenerateNewId(newPatient);
        newPatient.Id = _context.Patients.ToList().Count == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;

        // Insert the new patient into the database
        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();

        // Add the patient to the nurse queue
        var nurseQueueNo = GetNextQueueNumber("Nurse");
        var nurseQueue = new Queue
        {
            PatientNo = newPatient.PatientNo,
            QueueNo = nurseQueueNo,
            Status = "Nurse",
            DateToday = DateTime.Now
        };
        _context.Queues.Add(nurseQueue);

        await _context.SaveChangesAsync();

            TempData["ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {nurseQueueNo}";
            
        // Redirect to RecordsClerk action after successful creation
        return RedirectToAction(nameof(RecordsClerk));
    }

    public string GenerateNewId(Patient patient)
    {
        DateTime currentDate = DateTime.Now;
        char[] name = patient.LastName.ToCharArray();
        string id = $"HMS-{currentDate.Month}{currentDate.Day}-{currentDate.Year}-" +
            $"{name[0].ToString().ToUpper()}";
        int newId = _context.Patients.ToList().Count == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;
        if (newId < 10)
        {
            id += $"00{newId}";
        }
        else if (newId < 100)
        {
            id += $"0{newId}";
        }
        else
        {
            id += $"{newId}";
        }
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
                // Do something with patient details
                // e.g., return a view with patient details
                return View("RecordsClerk", patient);
            }
        }

        // Handle case where patient details are not found
        return RedirectToAction(nameof(RecordsClerk));
    }

    [HttpGet]
    // [Authorize(Roles = "Nursing")]
    public IActionResult Nurse(string? patientNo)
    {
        var nextPatientInLine = GetNextPatientInLine("Nurse");
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
            return View(vitalModel);
        }

        if (nextPatientInLine != null)
        {
            var vitalModel = new Vital
            {
                PatientNo = nextPatientInLine?.PatientNo // Ensure nextPatientInLine is not null
            };
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

                // Automatically add the patient to the doctor queue with the next queue number
                var doctorQueueNo = GetNextQueueNumber("Doctor");
                var doctorQueue = new Queue
                {
                    PatientNo = vital.PatientNo,
                    QueueNo = doctorQueueNo,
                    Status = "Doctor",
                    DateToday = DateTime.Now
                };
                RemovePatientFromQueue("Nurse", patient.PatientNo);
                _context.Queues.Add(doctorQueue);

                await _context.SaveChangesAsync();
                TempData["N_ConfirmationMessage"] = $"Patient's vitals added successfully. Patient's queue number is {doctorQueueNo} in the doctor queue.";
                return RedirectToAction(nameof(Nurse));
            }
        }
        TempData["N_WarningMessage"] = $"Error processing patient's vitals. Please try again";
        return RedirectToAction(nameof(Nurse));
    }
    [HttpGet]
    public IActionResult Lab(string? patientNo)
    {
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
            return View(labEntry);
        }


        return RedirectToAction(nameof(LabQueue));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lab([Bind("PatientNo, LabName, Result, Notes, Date")] Persol_HMS.Models.Lab lab)
    {
        if (!string.IsNullOrEmpty(lab.PatientNo) &&
            lab.LabName != null &&
            lab.Result != null &&
            lab.Date != null &&
            lab.Notes != null)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == lab.PatientNo);
            if (patient != null)
            {

                _context.Add(lab);
                var labEntry = new Persol_HMS.Models.Lab
                {
                    PatientNo = lab.PatientNo,
                    LabName = lab.LabName,
                    Result = lab.Result,
                    Notes = lab.Notes,
                    Date = lab.Date,
                };
                RemovePatientFromQueue("Lab", patient.PatientNo);
                _context.Labs.Add(labEntry);
                await _context.SaveChangesAsync();
                TempData["ConfirmationMessage"] = $"Patient's lab added successfully. " +
                    $"Patients Queue number is {labEntry.PatientNo}";
                return RedirectToAction(nameof(Doctor));
            }
            
        }
        TempData["WarningMessage"] = $"Error processing patient's. Please try again";
        return RedirectToAction(nameof(Lab));
    }

    public IActionResult NurseQueue(int page = 1, string search = "")
    {
        int pageSize = 10;

        var patientsInLine = _context.Queues
            .Where(q => q.Status == "Nurse" && q.PatientNo.Contains(search))
            .OrderBy(q => q.DateToday)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPatients = _context.Queues.Count(q => q.Status == "Nurse" && q.PatientNo.Contains(search));

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };

        return View(model);
    }
    public IActionResult LabQueue(int page = 1, string search = "")
    {
        int pageSize = 10;

        var patientsInLine = _context.Queues
            .Where(q => q.Status == "Lab" && q.PatientNo.Contains(search))
            .OrderBy(q => q.DateToday) // Order by the time they entered the queue
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPatients = _context.Queues.Count(q => q.Status == "Lab" && q.PatientNo.Contains(search));

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };

        return View(model);
    }

    public IActionResult DoctorQueue(int page = 1, string search = "")
    {
        int pageSize = 10;

        var patientsInLine = _context.Queues
            .Where(q => q.Status == "Doctor" && q.PatientNo.Contains(search))
            .OrderBy(q => q.DateToday)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPatients = _context.Queues.Count(q => q.Status == "Doctor" && q.PatientNo.Contains(search));

        var model = new QueueViewModel
        {
            PatientsInLine = patientsInLine,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPatients = totalPatients,
            Search = search
        };

        return View(model);
    }



    // Helper method to get the next patient in line for a given status
    private Patient GetNextPatientInLine(string status)
    {
        var nextPatientInLine = _context.Queues
            .Where(q => q.Status == status)
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



    // Helper method to get the next queue number for a given status
    private int GetNextQueueNumber(string status)
    {
        var maxQueueNumber = _context.Queues.Where(q => q.Status == status).Max(q => (int?)q.QueueNo) ?? 0;
        return maxQueueNumber + 1;
    }
}
