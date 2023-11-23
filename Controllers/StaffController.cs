using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                var medicalViewModel = new CreateMedicalViewModel{
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
                var medicalViewModel = new CreateMedicalViewModel{
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
                Diagnoses = model.Diagnoses,
                WardNo = model.WardNo,
                IsAdmitted = model.IsAdmitted,
                DateAdmitted = model.DateAdmitted ?? DateTime.Now
            };

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.PatientNo));
            var vital = await _context.Vitals.FirstOrDefaultAsync(v => v.PatientNo.Equals(model.PatientNo));
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

            // Save entities to the database if they don't exist
            if (patient != null)
            {
                medicalRecord.Patient = patient;
            }

            if (vital != null)
            {
                medicalRecord.Vital = vital;
            }

            _context.Medicals.Add(medicalRecord);
            await _context.SaveChangesAsync();

            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync();

            _context.Symptoms.Add(symptom);
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

            TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {nurseQueueNo} in the nurse queue.";
            
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

        public IActionResult Lab()
        {
            var patientsInQueue = GetNextPatientInLine("Lab");

            if (patientsInQueue != null)
            {
                var labEntry = new Lab
                {
                    PatientNo = patientsInQueue.PatientNo
                };
                return View(patientsInQueue);
            }

            return View(new Lab());
        }

        public IActionResult ProcessLabPatient(string patientNo)
        {
            var labEntry = new Lab
            {
                PatientNo = patientNo,
                LabName = "YourLabName",
                Result = false,
                Notes = "YourNotes",
                Date = DateTime.Now
            };

            _context.Labs.Add(labEntry);
            _context.SaveChanges();

            // TODO => Queue for lab ========
            // ==============================
            // ==============================

            // Redirect back to the lab index
            return RedirectToAction(nameof(Index));
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
