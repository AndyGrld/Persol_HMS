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

        else if (nextPatientInLine != null)
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

        // Handle case where there are no patients in the doctor's queue
        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveMedicalRecords(CreateMedicalViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Map the properties from the view model to your data models
            var medicalRecord = new Medical
            {
                PatientNo = model.PatientNo,
                Diagnoses = model.Diagnoses,
                WardNo = model.WardNo,
                IsAdmitted = model.IsAdmitted,
                DateAdmitted = model.DateAdmitted ?? DateTime.Now
                // Add other properties as needed
            };

            // Retrieve the foreign key IDs
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.PatientNo));
            var vital = await _context.Vitals.FirstOrDefaultAsync(v => v.PatientNo.Equals(model.PatientNo));
            var drug = new Drug
            {
                PatientNo = model.PatientNo,
                DrugName = model.DrugName,
                Dosage = model.Dosage,
                Date = DateTime.Now
                // Add other properties as needed
            };
            var symptom = new Symptom
            {
                PatientNo = model.PatientNo,
                Symptoms = model.Symptoms,
                Date = DateTime.Now
                // Add other properties as needed
            };
            var lab = new Lab
            {
                PatientNo = model.PatientNo,
                LabName = "Lab Test", // Adjust accordingly
                Result = false, // Adjust accordingly
                Notes = "Lab Notes", // Adjust accordingly
                Date = DateTime.Now
                // Add other properties as needed
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

            _context.Labs.Add(lab);
            await _context.SaveChangesAsync();

            // Remove the patient from the doctor queue since the doctor has attended to them
            RemovePatientFromQueue("Doctor", model.PatientNo);

            // Redirect to a success page or take further actions
            return RedirectToAction("Success");
        }

        // If the model is not valid, return to the same page to display validation errors
        return View("Doctor", model);
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
                // Existing patient logic here

                // Assuming you want to redirect to Nurse action
                return RedirectToAction(nameof(Nurse));
            }

            ModelState.AddModelError("PatientNo", "Patient not found");
        }
            // Generate and set the patient ID
            int id = _context.Patients.ToList().Count == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;
            newPatient.PatientNo = "HMS-1121-2023-K01" + id.ToString();
            newPatient.Id = id;

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

            TempData["ConfirmationMessage"] = "Patient created successfully.";
            
        // Redirect to RecordsClerk action after successful creation
        return RedirectToAction(nameof(RecordsClerk));
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
                _context.Vitals.Add(vital);

                // Automatically add the patient to the doctor queue with the next queue number
                var doctorQueueNo = GetNextQueueNumber("Doctor");
                var doctorQueue = new Queue
                {
                    PatientNo = vital.PatientNo,
                    QueueNo = doctorQueueNo,
                    Status = "Doctor",
                    DateToday = DateTime.Now
                };
                _context.Queues.Add(doctorQueue);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Nurse));
            }
        }
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
            .OrderBy(q => q.DateToday) // Order by the time they entered the queue
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
            .OrderBy(q => q.DateToday) // Order by the time they entered the queue
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
