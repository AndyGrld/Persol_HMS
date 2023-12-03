using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Lab = Persol_HMS.Models.Lab;

// [Authorize]
public class StaffController : Controller
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetDepartmentId()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
        if(user == null)
        {
            return 1;
        }
        return user.DepartmentId;
    }

    private IActionResult RedirectToHome()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Doctor(string? patientNo)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 3)
        // {
        //     return RedirectToHome();
        // }

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

            if (nextPatientInLine != null)
            {
                var medicalViewModel = new CreateMedicalViewModel
                {
                    PatientNo = nextPatientInLine.PatientNo,
                };

                ViewBag.Name = nextPatientInLine?.FirstName;
                var doctorQueue = Queue.GetOrCreateQueue(_context, nextPatientInLine.PatientNo, DepartmentType.Doctor);
                return View(medicalViewModel);
            }
        }

        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpPost]
    public async Task<IActionResult> SaveMedicalRecords(DoctorQueueModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 3)
        // {
        //     return RedirectToHome();
        // }

        if (!string.IsNullOrEmpty(model.CreateMedicalViewModel.PatientNo) &&
            model.CreateMedicalViewModel.Diagnoses != null &&
            model.CreateMedicalViewModel.DrugNames != null &&
            model.CreateMedicalViewModel.Symptoms != null)
        {
            var symptoms = new Symptom
            {
                ID = _context.Symptoms.Count() == 0 ? 1 : _context.Symptoms.Max(s => s.ID) + 1,
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                Symptoms = model.CreateMedicalViewModel.Symptoms,
                Date = DateTime.Now.Date,
            };

            _context.Symptoms.Add(symptoms);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.CreateMedicalViewModel.PatientNo));
            var vital = await _context.Vitals.OrderBy(l => l.Id).LastOrDefaultAsync(v => v.PatientNo.Equals(model.CreateMedicalViewModel.PatientNo));

            var medicalRecord = new Medical
            {
                ID = _context.Medicals.Count() == 0 ? 1 : _context.Medicals.Max(s => s.ID) + 1,
                PatientNo = model.CreateMedicalViewModel.PatientNo,
                Date = DateTime.Today,
                Diagnoses = model.CreateMedicalViewModel.Diagnoses,
                WardName = model.CreateMedicalViewModel.IsAdmitted == true ? model.CreateMedicalViewModel.SelectedWardNames[0] : null,
                IsAdmitted = model.CreateMedicalViewModel.IsAdmitted,
                DateAdmitted = model.CreateMedicalViewModel.IsAdmitted == true ? DateTime.Now.Date : (DateTime?)null
            };

            if (symptoms != null)
            {
                medicalRecord.SymptomsID = symptoms.ID;
                medicalRecord.Symptom = symptoms;
            }

            if (patient != null)
            {
                medicalRecord.Patient = patient;
                medicalRecord.PatientNo = patient.PatientNo;
            }

            if (vital != null)
            {
                medicalRecord.Vital = vital;
                medicalRecord.VitalsID = vital.Id;
            }

            _context.Medicals.Add(medicalRecord);
            await _context.SaveChangesAsync();

            for (int i = 0; i < model.CreateMedicalViewModel.DrugNames.Count; i++)
            {
                if(!string.IsNullOrEmpty(model.CreateMedicalViewModel.DrugNames[i].DrugName) &&
                    !string.IsNullOrEmpty(model.CreateMedicalViewModel.DrugNames[i].Dosage)){
                    var drug = new Drug
                    {
                        MedicalID = medicalRecord.ID,
                        ID = _context.Drugs.Count() == 0 ? 1 : _context.Drugs.Max(d => d.ID) + 1,
                        PatientNo = model.CreateMedicalViewModel.PatientNo,
                        DrugName = model.CreateMedicalViewModel.DrugNames[i].DrugName,
                        Dosage = model.CreateMedicalViewModel.DrugNames[i].Dosage,
                        Date = DateTime.Today
                    };

                    _context.Drugs.Add(drug);
                    await _context.SaveChangesAsync();
                }
            }


            if (model.CreateMedicalViewModel.NeedsLab && model.CreateMedicalViewModel.SelectedLabNames.Count > 0)
            {
                foreach (var labName in model.CreateMedicalViewModel.SelectedLabNames)
                {
                    var labQueueNo = GetNextQueueNumber("Lab");
                    var labQueue = new Queue
                    {
                        LabName = labName,
                        PatientNo = model.CreateMedicalViewModel.PatientNo,
                        QueueNo = labQueueNo,
                        Status = "Lab",
                        DateCreated = DateTime.Now
                    };

                    _context.Queues.Add(labQueue);
                    await _context.SaveChangesAsync();
                }
            }
            RemovePatientFromQueue("Doctor", model.CreateMedicalViewModel.PatientNo);


            TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully.";
            return RedirectToAction(nameof(DoctorQueue));
        }

        TempData["D_WarningMessage"] = $"Error processing patient's medical details. Please try again";
        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpGet]
    public async Task<IActionResult> RecordsClerk()
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 1)
        // {
        //     return RedirectToHome();
        // }
        return View(new Patient());
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrGetPatient([Bind("PatientNo, FirstName, LastName, DateOfBirth, ContactNo, InsuranceType, InsuranceNo, Gender, EmergencyContactFirstName, EmergencyContactLastName, EmergencyContactNo")] Patient newPatient,
        string patientNo = "")
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 1)
        // {
        //     return RedirectToHome();
        // }

        if (newPatient.PatientNo != null || !string.IsNullOrEmpty(patientNo))
        {
            var patient = newPatient.PatientNo != null 
                ? await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == newPatient.PatientNo)
                : await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == patientNo);
            if (patient != null)
            {
                bool patientInQueue = await _context.Queues.FirstOrDefaultAsync(q => q.PatientNo == patient.PatientNo) != null ? true : false;
                if (patientInQueue)
                {
                    TempData["R_ConfirmationMessage"] = $"Patient already in Queue";
                    return RedirectToAction(nameof(RecordsClerk));
                }
                var NurseQueueNo = GetNextQueueNumber("Nurse");
                var NurseQueue = new Queue
                {
                    PatientNo = patient.PatientNo,
                    QueueNo = NurseQueueNo,
                    Status = "Nurse",
                    DateCreated = DateTime.Now
                };
                _context.Queues.Add(NurseQueue);

                await _context.SaveChangesAsync();

                TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Queue number is {NurseQueueNo}";
                return RedirectToAction(nameof(RecordsClerk));
            }
            TempData["R_WarningMessage"] = $"Patient not found";
            return RedirectToAction(nameof(RecordsClerk));
        }
        else if(newPatient.FirstName != null)
        {
            newPatient.PatientNo = GenerateNewId(newPatient);
            newPatient.Id = _context.Patients.Count() == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patients Id is {newPatient.PatientNo}";
            return RedirectToAction(nameof(RecordsClerk));
        }
        TempData["R_WarningMessage"] = $"Please enter a value";
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
    //[ValidateAntiForgeryToken]
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
    public IActionResult Nurse(string? patientNo)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 2)
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
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Nurse([Bind("PatientNo, Temperature, Height, Weight, BloodPressure")] Vital vital)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 2)
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
                vital.Date = DateTime.Now;
                _context.Vitals.Add(vital);

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
    public async Task<IActionResult> Lab(LabQueueViewModel labView)
    {
        if (!string.IsNullOrEmpty(labView.Lab.PatientNo))
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == labView.Lab.PatientNo);

            if (patient != null)
            {
                var medical = await _context.Medicals.OrderBy(m => m.ID).LastOrDefaultAsync(m => m.PatientNo == labView.Lab.PatientNo);

                var lab = new Persol_HMS.Models.Lab
                {
                    ID = _context.Labs.Count() == 0 ? 1 : _context.Labs.Max(s => s.ID) + 1,
                    PatientNo = labView.Lab.PatientNo,
                    LabName = labView.Lab.LabName,
                    Result = labView.Lab.Result,
                    Notes = labView.Lab.Notes,
                    Date = DateTime.Today,
                    MedicalID = medical.ID
                };
                _context.Labs.Add(lab);
                await _context.SaveChangesAsync();

                var pharmacyQueueNo = GetNextQueueNumber("Pharmacy");
                var pharmacyQueue = new Queue
                {
                    PatientNo = labView.Lab.PatientNo,
                    QueueNo = pharmacyQueueNo,
                    Status = "Pharmacy",
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Lab", patient.PatientNo);
                _context.Queues.Add(pharmacyQueue);

                await _context.SaveChangesAsync();

                TempData["L_ConfirmationMessage"] = $"Patient's lab added successfully, patient may visit pharmacist to take drugs.";
                return RedirectToAction(nameof(LabQueue));
            }
        }

        TempData["L_WarningMessage"] = "Error processing patient's lab. Please try again";
        return RedirectToAction(nameof(LabQueue));
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> PatientLab(LabsViewModel labView)
    {
        if (!string.IsNullOrEmpty(labView.PatientNo) && labView.Lab != null)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == labView.PatientNo);

            if (patient != null)
            {
                var medical = await _context.Medicals.OrderBy(m => m.ID).LastOrDefaultAsync(m => m.PatientNo == labView.PatientNo);
                
                    var lab = new Persol_HMS.Models.Lab
                    {
                        ID = _context.Labs.Count() == 0 ? 1 : _context.Labs.Max(s => s.ID) + 1,
                        PatientNo = labView.Lab.PatientNo,
                        LabName = labView.Lab.LabName,
                        Result = labView.Lab.Result,
                        Notes = labView.Lab.Notes,
                        Date = DateTime.Today,
                        MedicalID = medical.ID
                    };
                    _context.Labs.Add(lab);
                    await _context.SaveChangesAsync();
                
                RemovePatientFromQueue("Lab", patient.PatientNo);
                TempData["L_ConfirmationMessage"] = $"Patient's lab added successfully, patient may visit pharmacist to take drugs.";
                return RedirectToAction(nameof(LabQueue));
            }
        }
        TempData["L_WarningMessage"] = "Error processing patient's lab. Please try again";
        return RedirectToAction(nameof(LabQueue));
    }


    public IActionResult NurseQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 2)
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;
        // 0987654321
        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Nurse" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        System.Console.WriteLine($"query : {query.Count()}");
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
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 4)
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Lab" &&
                        (q.PatientNo.Contains(search.ToUpper()) ||
                        q.Patient.FirstName.Contains(search.Titleize()) ||
                        q.Patient.LastName.Contains(search.Titleize())))
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

    public IActionResult Lab(string? patientNo)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 4)
        // {
        //     return RedirectToHome();
        // }
        if (patientNo != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
            var labEntry = new LabsViewModel
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
            var labEntry = new LabsViewModel
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
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 3)
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Doctor" &&
                        (q.PatientNo.Contains(search.ToUpper()) ||
                        q.Patient.FirstName.Contains(search.Titleize()) ||
                        q.Patient.LastName.Contains(search.Titleize())))
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

    [HttpGet]
    public IActionResult PatientList(int page = 1, string search = "")
    {
		ViewBag.deptId = GetDepartmentId();
        int pageSize = 10;
        var patients = _context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            patients = patients.Where(p =>
                                p.PatientNo.Contains(search.ToUpper()) ||
                                p.FirstName.Contains(search.Titleize()) ||
                                p.LastName.Contains(search.Titleize()))
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

    [HttpGet]
    public IActionResult PatientListOnly(int page = 1, string search = "")
    {
		ViewBag.deptId = GetDepartmentId();
        int pageSize = 10;
        var patients = _context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            patients = patients.Where(p =>
                                                    // .ToUpper()
                                p.PatientNo.Contains(search.ToUpper()) ||
                                p.FirstName.Contains(search.Titleize()) ||
                                p.LastName.Contains(search.Titleize()))
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

    [HttpGet]
    public IActionResult PatientMedicalRecords(string patientNo)
    {
		ViewBag.deptId = GetDepartmentId();
        var patient = _context.Patients
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Vital)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Symptom)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Drugs)
            .Include(p => p.Medicals)
                .ThenInclude(m => m.Labs)
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
            Drugs = m.Drugs,
            Labs = m.Labs
        }).ToList();

        var viewModel = new PatientMedicalViewModel
        {
            Patient = patient,
            MedicalRecords = medicalViewModels
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SavePatientMedicals(CreateMedicalViewModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 3)
        // {
        //     return RedirectToHome();
        // }
        if (!string.IsNullOrEmpty(model.PatientNo) && model.Diagnoses != null &&
            model.DrugNames.Count() > 1 && model.Symptoms != null)
        {

            var symptoms = new Symptom
            {
                ID = _context.Symptoms.Count() == 0 ? 1 : _context.Symptoms.Max(s => s.ID) + 1,
                PatientNo = model.PatientNo,
                Symptoms = model.Symptoms,
                Date = DateTime.Now.Date,

            };
            _context.Symptoms.Add(symptoms);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(model.PatientNo));
            var vital = await _context.Vitals.OrderBy(l => l.Id).LastOrDefaultAsync(v => v.PatientNo.Equals(model.PatientNo));

            var medicalRecord = new Medical
            {
                ID = _context.Medicals.Count() == 0 ? 1 : _context.Medicals.Max(s => s.ID) + 1,
                PatientNo = model.PatientNo,
                Date = DateTime.Today,
                Diagnoses = model.Diagnoses,
                WardName = model.IsAdmitted == true ? model.SelectedLabNames[0] : null,
                IsAdmitted = model.IsAdmitted,
                DateAdmitted = model.IsAdmitted == true ? DateTime.Now.Date : (DateTime?)null
            };

            if (symptoms != null)
            {
                medicalRecord.SymptomsID = symptoms.ID;
                medicalRecord.Symptom = symptoms;
            }

            if (patient != null)
            {
                medicalRecord.Patient = patient;
                medicalRecord.PatientNo = patient.PatientNo;
            }

            if (vital != null)
            {
                medicalRecord.Vital = vital;
                medicalRecord.VitalsID = vital.Id;
            }
            _context.Medicals.Add(medicalRecord);
            await _context.SaveChangesAsync();

            for (int i = 0; i < model.DrugNames.Count; i++)
            {
                var drug = new Drug
                {
                    MedicalID = medicalRecord.ID,
                    ID = _context.Drugs.Count() == 0 ? 1 : _context.Drugs.Max(d => d.ID) + 1,
                    PatientNo = model.PatientNo,
                    DrugName = model.DrugNames[i].DrugName,
                    Dosage = model.DrugNames[i].Dosage,
                    Date = DateTime.Today
                };
                _context.Drugs.Add(drug);
                await _context.SaveChangesAsync();
            }

            var labQueueNo = GetNextQueueNumber("Lab");
            var labQueue = new Queue
            {
                PatientNo = model.PatientNo,
                QueueNo = labQueueNo,
                Status = "Lab",
                DateCreated = DateTime.Now
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
    public IActionResult PharmacyQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 6)
        // {
        //     return RedirectToHome();
        // }
        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Pharmacy" &&
                        (q.PatientNo.Contains(search.ToUpper()) ||
                        q.Patient.FirstName.Contains(search.Titleize()) ||
                        q.Patient.LastName.Contains(search.Titleize())))
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

        var viewModel = new PharmacyQueueViewModel
        {
            QueueViewModel = model,
            PatientsWithDrugs = patientsInLine.Select(patient =>
                new PatientWithDrugs
                {
                    PatientQueue = patient,
                    PatientDrugs = GetDrugsFromLatestMedicalRecord(patient.PatientNo)
                }).ToList()
        };
        return View(viewModel);
    }

    private List<Drug> GetDrugsFromLatestMedicalRecord(string patientId)
    {
        var latestMedicalRecord = _context.Medicals
            .Where(m => m.Patient.PatientNo == patientId)
            .OrderByDescending(m => m.Date)
            .Include(m => m.Drugs)
            .FirstOrDefault();
        return latestMedicalRecord?.Drugs.ToList() ?? new List<Drug>();
    }

    [HttpPost]
    public IActionResult UpdateDrugPrice(PharmacyQueueViewModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 6)
        // {
        //     return RedirectToHome();
        // }
        if (model.PatientsWithDrugs != null)
        {
            var patientWithDrugs = model.PatientsWithDrugs[0];
            if (patientWithDrugs != null)
            {
                double bill = 0;
                Drug drugToUpdate = new Drug();
                foreach (var drug in patientWithDrugs.PatientDrugs)
                {
                    drugToUpdate = _context.Drugs.FirstOrDefault(d => d.ID == drug.ID);
                    drugToUpdate.Price = drug.Price;
                    bill += drug.Price;
                }
                _context.SaveChanges();

                string PatientNo = patientWithDrugs.PatientQueue.PatientNo;
                var medical = _context.Medicals.FirstOrDefault(d => d.ID == drugToUpdate.MedicalID);
                medical.Bill += bill;

                var cashierQueueNo = GetNextQueueNumber("Cashier");
                var cashierQueue = new Queue
                {
                    PatientNo = PatientNo,
                    QueueNo = cashierQueueNo,
                    Status = "Cashier",
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Pharmacy", PatientNo);
                _context.Queues.Add(cashierQueue);
                _context.SaveChanges();

                TempData["P_ConfirmationMessage"] = "Drug prices updated successfully. Patient may visit cashier to pay bills.";
                return RedirectToAction("PharmacyQueue");
            }
            else
            {
                TempData["P_WarningMessage"] = "Patient not found in the queue.";
            }
        }

        TempData["P_WarningMessage"] = "Model state not valid";
        return RedirectToAction("PharmacyQueue");
    }

    public async Task<IActionResult> CashierQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 7)
        // {
        //     return RedirectToHome();
        // }

        int pageSize = 10;

        var query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Cashier" &&
                        (q.PatientNo.Contains(search.ToUpper()) ||
                        q.Patient.FirstName.Contains(search.Titleize()) ||
                        q.Patient.LastName.Contains(search.Titleize())))
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
        var viewModel = new CashierQueueViewModel
        {
            QueueViewModel = model,
            PatientsWithLatestMedical = (await Task.WhenAll(patientsInLine.Select(async patient =>
                new PatientWithLatestMedical
                {
                    PatientQueue = patient,
                    latestMedical = await GetLatestMedicalRecord(patient.PatientNo)
                })))
            .ToList()
        };
        return View(viewModel);
    }

    private async Task<Medical> GetLatestMedicalRecord(string patientNo){
        var latestMedical = await _context.Medicals
            .Where(m => m.PatientNo == patientNo)
            .OrderByDescending(m => m.Date)
            .FirstOrDefaultAsync();
        return latestMedical ?? new Medical();
    }

    public async Task<IActionResult> ConfirmPayment(CashierQueueViewModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        // if (ViewBag.deptId != 7)
        // {
        //     return RedirectToHome();
        // }
        if (model.PatientsWithLatestMedical != null)
        {
            var patientWithLatestMedical = model.PatientsWithLatestMedical[0];
            if (patientWithLatestMedical != null)
            {
                string PatientNo = patientWithLatestMedical.PatientQueue.PatientNo;
                var medicalToUpdate = await _context.Medicals.
                    FirstOrDefaultAsync(m => m.ID == patientWithLatestMedical.latestMedical.ID);
                medicalToUpdate.isPaid = true;
                RemovePatientFromQueue("Cashier", PatientNo);
                _context.SaveChanges();

                TempData["C_ConfirmationMessage"] = "Data has been saved successfully, patient may leave.";
                return RedirectToAction("CashierQueue");
            }
            else
            {
                TempData["C_WarningMessage"] = "Patient not found in the queue.";
            }
        }

        TempData["C_WarningMessage"] = "Model state not valid";
        return RedirectToAction("CashierQueue");
    }
}
