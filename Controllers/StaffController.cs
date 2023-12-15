using System;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Lab = Persol_HMS.Models.Lab;

[Authorize]
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
            return 3;
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
        if (ViewBag.deptId != 3)
        {
            return RedirectToHome();
        }

        Patient nextPatientInLine = new Patient();
        if (patientNo != null)
        {
            nextPatientInLine = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
        }
        else{
            nextPatientInLine = GetNextPatientInLine("Doctor");
        }

        if(nextPatientInLine != null){
            patientNo = nextPatientInLine.PatientNo;

            int pageSize = 10;
            int page = 1;

            var query = _context.Queues.AsQueryable();

            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.PatientNo == patientNo)
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

            var patientsInLine = query.ToList();
            var totalPatients = query.Count();

            List<CreateMedicalViewModel> createMedicalViews = new List<CreateMedicalViewModel>();

            foreach(var patient in patientsInLine)
            {
                CreateMedicalViewModel createMedicalViewModel = new();
                if(patient.HasVisitedLab)
                {
                    var latestMedical = _context.Medicals
                        .Include(m => m.Drugs)
                        .Include(m => m.Symptom)
                        .OrderByDescending(m => m.ID)
                        .FirstOrDefault(m => m.PatientNo == patient.PatientNo);

                    var labs = _context.Labs.Where(l => l.MedicalID == latestMedical.ID).ToList();

                    // mapping values
                    createMedicalViewModel.Symptoms = latestMedical.Symptom.Symptoms;
                    createMedicalViewModel.Diagnoses = latestMedical.Diagnoses;
                    createMedicalViewModel.DrugNames = _context.Drugs.Where(d  => d.MedicalID == latestMedical.ID).ToList();
                    createMedicalViewModel.visitedLabs = labs;
                    createMedicalViewModel.DateAdmitted = latestMedical.DateAdmitted;

                    createMedicalViews.Add(createMedicalViewModel);
                }
                else
                {
                    createMedicalViews.Add(createMedicalViewModel);
                }
            }

            var model = new QueueViewModel
            {
                PatientsInLine = patientsInLine,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPatients = totalPatients,
                Search = ""
            };

            var viewModel = new DoctorQueueModel
            {
                CreateMedicalViewModel = createMedicalViews,
                QueueViewModel = model
            };

            return View(viewModel);
        }
        return RedirectToAction(nameof(DoctorQueue));
    }

    [HttpGet]
    public IActionResult AdmittedQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        
        int pageSize = 10;
        var patients = _context.AdmittedPatients.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            patients = patients.Where(ap =>
                                ap.PatientNo.Contains(search.ToUpper()) ||
                                ap.WardName.Contains(search.ToUpper()))
                                .OrderBy(q => q.PatientNo)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize);
        }
        var model = new AdmittedViewModel{
            TotalPatients = _context.Patients.Count(),
            PageSize = pageSize,
            CurrentPage = page,
            Search = search,
            Patients = patients.ToList()
        };

        return View(model);
    }

    public IActionResult DoctorQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 3)
        {
            return RedirectToHome();
        }

        int pageSize = 10;

        var query = _context.Queues.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Doctor" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }else{
            query = _context.Queues
                .Include(q => q.Patient)
                .Where(q => q.Status == "Doctor")
                .OrderBy(q => q.QueueNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        var patientsInLine = query.ToList();
        var totalPatients = query.Count();

        List<CreateMedicalViewModel> createMedicalViews = new List<CreateMedicalViewModel>();

        foreach(var patient in patientsInLine)
        {
            CreateMedicalViewModel createMedicalViewModel = new();
            if(patient.HasVisitedLab)
            {
                var latestMedical = _context.Medicals
                    .Include(m => m.Drugs)
                    .Include(m => m.Symptom)
                    .OrderByDescending(m => m.ID)
                    .FirstOrDefault(m => m.PatientNo == patient.PatientNo);

                var labs = _context.Labs.Where(l => l.MedicalID == latestMedical.ID).ToList();

                // mapping values
                createMedicalViewModel.Symptoms = latestMedical.Symptom.Symptoms;
                createMedicalViewModel.Diagnoses = latestMedical.Diagnoses;
                createMedicalViewModel.DrugNames = _context.Drugs.Where(d  => d.MedicalID == latestMedical.ID).ToList();
                createMedicalViewModel.visitedLabs = labs;
                createMedicalViewModel.DateAdmitted = latestMedical.DateAdmitted;

                createMedicalViews.Add(createMedicalViewModel);
            }
            else
            {
                createMedicalViews.Add(createMedicalViewModel);
            }
        }

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
            CreateMedicalViewModel = createMedicalViews,
            QueueViewModel = model
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Discharge(string patientId, decimal billAmount)
    {
        try
        {
            // Retrieve the patient's latest medical record
            var latestMedical = _context.Medicals
                .Include(m => m.Drugs)
                .Include(m => m.Symptom)
                .OrderByDescending(m => m.Date)
                .FirstOrDefault(m => m.PatientNo == patientId);
            

            var dailyData = await _context.DailyDatas.FirstOrDefaultAsync(dd => dd.Date.Date == DateTime.Now.Date);
            if(dailyData == null)
            {
                dailyData = new DailyData()
                {
                    Date = DateTime.Now.Date
                };
                _context.DailyDatas.Add(dailyData);
                _context.SaveChanges();
            }

            if (latestMedical != null)
            {
                latestMedical.Bill += (double)billAmount;

                _context.Medicals.Update(latestMedical);

                var DoctorQueueNo = GetNextQueueNumber("Doctor");
                var DoctorQueue = new Queue
                {
                    PatientNo = patientId,
                    QueueNo = DoctorQueueNo,
                    Status = "Doctor",
                    DateCreated = DateTime.Now,
                    HasVisitedLab = true
                };
                _context.Queues.Add(DoctorQueue);

                var admittedPatient = _context.AdmittedPatients.FirstOrDefault(p => p.PatientNo == latestMedical.PatientNo);
                if (admittedPatient != null)
                {
                    _context.AdmittedPatients.Remove(admittedPatient);
                }
                dailyData.TotalDischarged += 1;
                dailyData.WardProfit += (double)billAmount;
                _context.DailyDatas.Update(dailyData);

                await _context.SaveChangesAsync();
                TempData["A_ConfirmationMessage"] = $"Patient may visit doctor for final treatment.";
                return RedirectToAction(nameof(AdmittedQueue));
            }

            TempData["A_WarningMessage"] = $"Patient medical record not found.";
            return RedirectToAction(nameof(AdmittedQueue));
        }
        catch (Exception ex)
        {
            TempData["A_WarningMessage"] = $"An error occured while discharging patient, please try again.";
            return RedirectToAction(nameof(AdmittedQueue));
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveMedicalRecords(DoctorQueueModel model, List<string> SelectLabNames, List<string> SelectWardNames)
    {
        try {
            ViewBag.deptId = GetDepartmentId();
            if (ViewBag.deptId != 3)
            {
                return RedirectToHome();
            }


            var saveModel = model.CreateMedicalViewModel[0];

            var dailyData = await _context.DailyDatas.FirstOrDefaultAsync(dd => dd.Date.Date == DateTime.Now.Date);
            if (dailyData == null)
            {
                dailyData = new DailyData()
                {
                    Date = DateTime.Now.Date
                };
                _context.DailyDatas.Add(dailyData);
                _context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(saveModel.PatientNo) &&
                saveModel.Diagnoses != null &&
                saveModel.Symptoms != null)
            {

                var patientfromQueue = await _context.Queues.FirstOrDefaultAsync(q => q.PatientNo == saveModel.PatientNo);
                var symptoms = new Symptom();
                var medicalRecord = new Medical();

                if (patientfromQueue.HasVisitedLab) {
                    medicalRecord = _context.Medicals
                        .Include(m => m.Drugs)
                        .Include(m => m.Symptom)
                        .OrderByDescending(m => m.Date)
                        .FirstOrDefault(m => m.PatientNo == saveModel.PatientNo);
                    symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.ID == medicalRecord.ID);
                    symptoms.Symptoms = saveModel.Symptoms;
                    _context.Symptoms.Update(symptoms);
                    _context.SaveChanges();
                }
                else {
                    symptoms.ID = _context.Symptoms.Count() == 0 ? 1 : _context.Symptoms.Max(s => s.ID) + 1;
                    symptoms.PatientNo = saveModel.PatientNo;
                    symptoms.Symptoms = saveModel.Symptoms;
                    symptoms.Date = DateTime.Now.Date;
                    _context.Symptoms.Add(symptoms);
                    _context.SaveChanges();
                };

                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo.Equals(saveModel.PatientNo));
                var vital = await _context.Vitals.OrderBy(l => l.Id).LastOrDefaultAsync(v => v.PatientNo.Equals(saveModel.PatientNo));

                bool isAdmitted = saveModel.SelectedWardNames.Count() > 0;

                if (!patientfromQueue.HasVisitedLab) {
                    medicalRecord.ID = _context.Medicals.Count() == 0 ? 1 : _context.Medicals.Max(s => s.ID) + 1;
                    medicalRecord.PatientNo = saveModel.PatientNo;
                    medicalRecord.Date = DateTime.Today;
                    medicalRecord.Diagnoses = saveModel.Diagnoses;
                    medicalRecord.IsAdmitted = isAdmitted;
                    medicalRecord.DateAdmitted = isAdmitted == true ? DateTime.Now.Date : null;

                    if (symptoms != null)
                    {
                        medicalRecord.SymptomsID = symptoms.ID;
                        medicalRecord.Symptom = symptoms;
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
                }
                else {
                    medicalRecord = medicalRecord;
                    medicalRecord.Diagnoses = saveModel.Diagnoses;
                    medicalRecord.IsAdmitted = isAdmitted;
                    medicalRecord.DateAdmitted = isAdmitted == true ? DateTime.Now.Date : null;
                    _context.Medicals.Update(medicalRecord);
                }
                _context.SaveChanges();

                // delete all old drugs
                var oldDrugs = _context.Drugs.Where(d => d.MedicalID == medicalRecord.ID).ToList();
                if (oldDrugs.Count() > 0) {
                    foreach (var oldDrug in oldDrugs) {
                        _context.Drugs.Remove(oldDrug);
                    }
                    _context.SaveChanges();
                }

                for (int i = 0; i < saveModel.DrugNames.Count; i++)
                {
                    if (!string.IsNullOrEmpty(saveModel.DrugNames[i].DrugName) &&
                        !string.IsNullOrEmpty(saveModel.DrugNames[i].Dosage)) {
                        var drug = new Drug
                        {
                            MedicalID = medicalRecord.ID,
                            ID = _context.Drugs.Count() == 0 ? 1 : _context.Drugs.Max(d => d.ID) + 1,
                            PatientNo = saveModel.PatientNo,
                            DrugName = saveModel.DrugNames[i].DrugName,
                            Dosage = saveModel.DrugNames[i].Dosage,
                            Date = DateTime.Today
                        };
                        _context.Drugs.Add(drug);
                        await _context.SaveChangesAsync();
                    }
                }

                // if admitted save status as IsAdmitted and forget about lab and drugs
                if (SelectWardNames.Count() > 0 && !string.IsNullOrEmpty(SelectWardNames[0]))
                {
                    medicalRecord.WardName = SelectWardNames[0];
                    var Admitted = new AdmittedPatient() {
                        ID = _context.AdmittedPatients.Count() == 0 ? 1 : _context.AdmittedPatients.Max(d => d.ID) + 1,
                        PatientNo = saveModel.PatientNo,
                        PatientName = patient.FirstName + " " + patient.LastName,
                        DateAdmitted = DateTime.Now,
                        WardName = SelectWardNames[0],
                        MedicalID = medicalRecord.ID
                    };
                    _context.AdmittedPatients.Add(Admitted);
                    dailyData.TotalAdmitted += 1;
                    _context.DailyDatas.Update(dailyData);
                    _context.SaveChanges();
                    TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient has been admitted.";
                }
                else if (SelectLabNames.Count() > 0 && !string.IsNullOrEmpty(SelectLabNames[0]))
                {
                    string labNames = "";
                    foreach (var labName in SelectLabNames)
                    {
                        labNames += " " + labName;
                    }
                    var labQueueNo = GetNextQueueNumber("Lab");
                    var labQueue = new Queue
                    {
                        LabName = labNames.Trim(),
                        PatientNo = saveModel.PatientNo,
                        QueueNo = labQueueNo,
                        Status = "Lab",
                        DateCreated = DateTime.Now
                    };
                    _context.Queues.Add(labQueue);
                    TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient is {labQueueNo} in lab queue.";
                }
                // not admitted or labs, but has drugs send to pharmacy to take drugs
                else if (saveModel.DrugNames.Count() > 0)
                {
                    if(!string.IsNullOrEmpty(saveModel.DrugNames[0].DrugName) &&
                        !string.IsNullOrEmpty(saveModel.DrugNames[0].Dosage)){
                        var PharmacyQueueNo = GetNextQueueNumber("Pharmacy");
                        var PharmacyQueue = new Queue
                        {
                            PatientNo = saveModel.PatientNo,
                            QueueNo = PharmacyQueueNo,
                            Status = "Pharmacy",
                            DateCreated = DateTime.Now
                        };
                        _context.Queues.Add(PharmacyQueue);
                        TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient is {PharmacyQueueNo} in pharmacy queue.";
                    }
                    else{
                        var CashierQueueNo = GetNextQueueNumber("Cashier");
                        var CashierQueue = new Queue
                        {
                            PatientNo = saveModel.PatientNo,
                            QueueNo = CashierQueueNo,
                            Status = "Cashier",
                            DateCreated = DateTime.Now
                        };
                        _context.Queues.Add(CashierQueue);
                        TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient is {CashierQueueNo} in cashier queue.";
                    }
                }
                else if (medicalRecord.Bill != 0) {
                    var CashierQueueNo = GetNextQueueNumber("Cashier");
                    var CashierQueue = new Queue
                    {
                        PatientNo = saveModel.PatientNo,
                        QueueNo = CashierQueueNo,
                        Status = "Cashier",
                        DateCreated = DateTime.Now
                    };
                    _context.Queues.Add(CashierQueue);
                    TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient is {CashierQueueNo} in cashier queue.";
                }
                // no lab, ward, drugs, can walk out, patient was fine all along (needs sleep)
                else
                {
                    TempData["D_ConfirmationMessage"] = $"Patient's medical details added successfully, patient may exit hospital.";
                }
                RemovePatientFromQueue("Doctor", saveModel.PatientNo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DoctorQueue));
            }

            TempData["D_WarningMessage"] = $"Error processing patient's medical details. Please try again";
            return RedirectToAction(nameof(DoctorQueue));
        }
        catch (Exception ex)
        {
            TempData["D_WarningMessage"] = $"An error occured while processing patient's medicals, please try again.";
            return RedirectToAction(nameof(AdmittedQueue));
        }
    }

    [HttpGet]
    public async Task<IActionResult> RecordsClerk()
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 1)
        {
            return RedirectToHome();
        }

        // Delete old patients with the status "IsDone"
        await DeleteOldPatients();

        // Check if patient is in the queue with status "IsDone"
        var isPatientInQueue = TempData["IsPatientInQueue"] as bool? ?? false;
        ViewBag.IsPatientInQueue = isPatientInQueue;

        return View(new Patient());
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrGetPatient([Bind("PatientNo, FirstName, LastName, DateOfBirth, ContactNo, InsuranceType, InsuranceExpireDate, InsuranceNo, Gender, EmergencyContactFirstName, EmergencyContactLastName, EmergencyContactNo")] Patient newPatient,
        string patientNo = "", string confirm= "")
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 1)
        {
            return RedirectToHome();
        }

        var dailyData = await _context.DailyDatas.FirstOrDefaultAsync(dd => dd.Date.Date == DateTime.Now.Date);
        if(dailyData == null)
        {
            dailyData = new DailyData()
            {
                Date = DateTime.Now.Date
            };
            _context.DailyDatas.Add(dailyData);
            _context.SaveChanges();
        }

        if (!string.IsNullOrEmpty(confirm) && !string.IsNullOrEmpty(patientNo))
        {
            var patientInQueue = await _context.Queues.FirstOrDefaultAsync(q => q.PatientNo == patientNo);
            _context.Queues.Remove(patientInQueue);
            var NurseQueueNo = GetNextQueueNumber("Nurse");
            var NurseQueue = new Queue
            {
                PatientNo = patientNo,
                QueueNo = NurseQueueNo,
                Status = "Nurse",
                DateCreated = DateTime.Now
            };
            _context.Queues.Add(NurseQueue);
            dailyData.TotalPatients += 1;
            _context.DailyDatas.Update(dailyData);
            await _context.SaveChangesAsync();

            TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patient's queue number is {NurseQueueNo} in nurse queue.";
            return RedirectToAction(nameof(RecordsClerk));
        }

        if (newPatient.PatientNo != null || !string.IsNullOrEmpty(patientNo))
        {
            var patient = newPatient.PatientNo != null 
                ? await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == newPatient.PatientNo)
                : await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == patientNo);
            if (patient != null)
            {
                var patientInQueue = await _context.Queues.OrderByDescending(p => p.Id).FirstOrDefaultAsync(q => q.PatientNo == patient.PatientNo && q.DateCreated.Date == DateTime.Now.Date);
                if (patientInQueue != null)
                {
                    if(patientInQueue.Status.Equals("IsDone")){
                        TempData["R_ConfirmationMessage"] = $"Patient has been here today";
                        TempData["IsPatientInQueue"] = patientInQueue != null && patientInQueue.Status.Equals("IsDone");
                        TempData["PatientNo"] = patientInQueue.PatientNo;
                        return RedirectToAction(nameof(RecordsClerk));
                    }
                    TempData["R_WarningMessage"] = $"Patient already in queue to visit {patientInQueue.Status}";
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
                dailyData.TotalPatients += 1;
                _context.DailyDatas.Update(dailyData);

                await _context.SaveChangesAsync();

                TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patient's queue number is {NurseQueueNo} in nurse queue.";
                return RedirectToAction(nameof(RecordsClerk));
            }
            TempData["R_WarningMessage"] = $"Patient not found";
            return RedirectToAction(nameof(RecordsClerk));
        }
        else if(newPatient.FirstName != null)
        {
            newPatient.PatientNo = GenerateNewId(newPatient);
            newPatient.Id = _context.Patients.Count() == 0 ? 1 : _context.Patients.Max(p => p.Id) + 1;
            newPatient.FirstName = newPatient.FirstName.Titleize();
            newPatient.LastName = newPatient.LastName.Titleize();
            newPatient.EmergencyContactLastName = newPatient.EmergencyContactLastName.Titleize();
            newPatient.EmergencyContactFirstName = newPatient.EmergencyContactFirstName.Titleize();

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();
            dailyData.NewPatients += 1;
            _context.DailyDatas.Update(dailyData);

            TempData["R_ConfirmationMessage"] = $"Patient created successfully. Patient's Id is {newPatient.PatientNo}";
            return RedirectToAction(nameof(RecordsClerk));
        }
        TempData["R_WarningMessage"] = $"Please enter a value";
        return RedirectToAction(nameof(RecordsClerk));
    }

    private async Task DeleteOldPatients()
    {
        var cutoffDate = DateTime.Now;
        var patientsToDelete = _context.Queues
            .Where(q => q.Status == "IsDone" && q.DateCreated.Date < cutoffDate.Date)
            .ToList();
        foreach (var patient in patientsToDelete)
        {
            _context.Queues.Remove(patient);
        }
        await _context.SaveChangesAsync();
    }

    public string GenerateNewId(Patient patient)
    {
        DateTime currentDate = DateTime.Now;
        char[] name = patient.LastName.ToCharArray();
        string idPrefix = $"HMS-{currentDate.Month:D2}{currentDate.Day:D2}-{currentDate.Year:D4}-" +
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
        if (ViewBag.deptId != 2)
        {
            return RedirectToHome();
        }
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
    public async Task<IActionResult> Nurse([Bind("PatientNo, Temperature, Height, Weight, BloodPressure")] Vital vital,
            string returnUrl = "")
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 2)
        {
            return RedirectToHome();
        }
        int n;
        if (!string.IsNullOrEmpty(vital.PatientNo) &&
            vital.Temperature != 0 && vital.BloodPressure.Contains("/") &&
            vital.Height != 0 && int.TryParse(vital.BloodPressure.Split("/")[0], out n) &&
            vital.Weight != 0 && int.TryParse(vital.BloodPressure.Split("/")[1], out n) &&
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

                if (!string.IsNullOrEmpty(returnUrl)) {
                    return RedirectToAction(nameof(Nurse));
                }
                return RedirectToAction(nameof(NurseQueue));
            }
        }
        TempData["N_WarningMessage"] = $"Error processing patient's vitals. Please try again";
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction(nameof(Nurse));
        }
        return RedirectToAction(nameof(NurseQueue));
    }


    [HttpPost]
    public async Task<IActionResult> Lab(LabQueueViewModel labView)
    {
        if (!string.IsNullOrEmpty(labView.Labs[0].PatientNo) && labView.Labs.Count > 0)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == labView.Labs[0].PatientNo);

            if (patient != null)
            {
                var medical = await _context.Medicals.OrderBy(m => m.ID).LastOrDefaultAsync(m => m.PatientNo == labView.Labs[0].PatientNo);
                
                var lab = new Persol_HMS.Models.Lab();
                foreach(var labTaken in labView.Labs)
                {
                    lab = new Persol_HMS.Models.Lab
                    {
                        ID = _context.Labs.Count() == 0 ? 1 : _context.Labs.Max(s => s.ID) + 1,
                        PatientNo = patient.PatientNo,
                        LabName = labTaken.LabName,
                        Notes = labTaken.Notes,
                        Result = labTaken.Result,
                        Date = DateTime.Today,
                        MedicalID = medical.ID
                    };
                    _context.Labs.Add(lab);
					await _context.SaveChangesAsync();
                }

                var DoctorQueueNo = GetNextQueueNumber("Doctor");
                var DoctorQueue = new Queue
                {
                    PatientNo = patient.PatientNo,
                    QueueNo = DoctorQueueNo,
                    Status = "Doctor",
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Lab", patient.PatientNo);
                _context.Queues.Add(DoctorQueue);

                await _context.SaveChangesAsync();

                TempData["L_ConfirmationMessage"] = $"Patient's lab added successfully, patient may go back to doctor for diagnosis.";
                return RedirectToAction(nameof(Lab));
            }
        }

        TempData["L_WarningMessage"] = "Error processing patient's lab. Please try again";
        return RedirectToAction(nameof(Lab));
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> PatientLab(LabsViewModel labView)
    {
        bool filledAtLeastOne = false;
        for (int i = 0; i < labView.Labs.Count(); i++)
        {
            if (!string.IsNullOrEmpty(labView.Labs[i].Result) && !string.IsNullOrEmpty(labView.Labs[i].Notes))
            {
                filledAtLeastOne = true;
                break;
            }
        }
        if (!string.IsNullOrEmpty(labView.patient.PatientNo) && labView.Labs.Count() > 0 && filledAtLeastOne)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == labView.patient.PatientNo);
            if (patient != null)
            {
                var medical = await _context.Medicals.OrderBy(m => m.ID).LastOrDefaultAsync(m => m.PatientNo == labView.patient.PatientNo);
                var lab = new Persol_HMS.Models.Lab();
                var queueDetails = await _context.Queues.FirstOrDefaultAsync(q => q.PatientNo == labView.patient.PatientNo);
                List<string> labNames = queueDetails.LabName.Split(" ").ToList();
                foreach(var labTaken in labView.Labs)
                {
                    if (!string.IsNullOrEmpty(labTaken.Result) && !string.IsNullOrEmpty(labTaken.Notes))
                    {
                        lab = new Persol_HMS.Models.Lab
                        {
                            ID = _context.Labs.Count() == 0 ? 1 : _context.Labs.Max(s => s.ID) + 1,
                            PatientNo = patient.PatientNo,
                            LabName = labTaken.LabName,
                            Notes = labTaken.Notes,
                            Result = labTaken.Result,
                            Date = DateTime.Today,
                            MedicalID = medical.ID
                        };
                        labNames.Remove(labTaken.LabName);
                        _context.Labs.Add(lab);
                        await _context.SaveChangesAsync();
                    }
                }

                if(labNames.Count > 0){
                    string remainingLabs = "";
                    foreach(var labName in labNames){
                        remainingLabs += " " + labName;
                    }
                    queueDetails.LabName = remainingLabs.Trim();;
                    _context.Queues.Update(queueDetails);
                    await _context.SaveChangesAsync();
                    TempData["L_ConfirmationMessage"] = $"Patient's lab updated successfully";
                    return RedirectToAction(nameof(Lab));
                }

                var DoctorQueueNo = GetNextQueueNumber("Doctor");
                var DoctorQueue = new Queue
                {
                    PatientNo = patient.PatientNo,
                    QueueNo = DoctorQueueNo,
                    Status = "Doctor",
                    HasVisitedLab = true,
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Lab", patient.PatientNo);
                _context.Queues.Add(DoctorQueue);

                await _context.SaveChangesAsync();

                TempData["L_ConfirmationMessage"] = $"Patient's lab added successfully, patient may go back to doctor for diagnosis.";
                return RedirectToAction(nameof(Lab));
            }
        }

        TempData["L_WarningMessage"] = "Error processing patient's lab. Please try again";
        return RedirectToAction(nameof(Lab));
    }


    public IActionResult NurseQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 2)
        {
            return RedirectToHome();
        }

        int pageSize = 10;

        var query = _context.Queues.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Nurse" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }else{
            query = _context.Queues
                .Include(q => q.Patient)
                .Where(q => q.Status == "Nurse")
                .OrderBy(q => q.QueueNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

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
        if (ViewBag.deptId != 4)
        {
            return RedirectToHome();
        }

        int pageSize = 10;

        var query = _context.Queues.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Lab" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }else{
            query = _context.Queues
                .Include(q => q.Patient)
                .Where(q => q.Status == "Lab")
                .OrderBy(q => q.QueueNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
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
            Labs = new List<Persol_HMS.Models.Lab>(),
            QueueViewModel = model
        };

        return View(viewModel);
    }

    public IActionResult Lab(string? patientNo)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 4)
        {
            return RedirectToHome();
        }
        if (patientNo != null)
        {
            var patientDetails = _context.Patients.FirstOrDefault(p => p.PatientNo == patientNo);
            var queueData = _context.Queues.FirstOrDefault(q => q.PatientNo == patientDetails.PatientNo && q.Status == "Lab");

            if(string.IsNullOrEmpty(queueData.LabName))
            {
                return RedirectToAction(nameof(LabQueue));
            }
            List<string> labNames = queueData.LabName.Split(" ").Where(lab => lab != " ").ToList();
            List<Lab> labsToTake = new List<Lab>();

            for(int i = 0; i < labNames.Count(); i++)
            {
                labsToTake.Add(new Lab(){
                    LabName = labNames[i]
                });
            }
            var labEntry = new LabsViewModel
            {
                patient = patientDetails,
                Labs = labsToTake
            };
            if (!string.IsNullOrEmpty(labEntry.patient.PatientNo))
            {
                var labQueue = Queue.GetOrCreateQueue(_context, labEntry.patient.PatientNo, DepartmentType.Lab);
            }
            return View(labEntry);
        }

        var patientsInQueue = GetNextPatientInLine("Lab");
        if (patientsInQueue != null)
        {
            var queueData = _context.Queues.FirstOrDefault(q => q.PatientNo == patientsInQueue.PatientNo && q.Status == "Lab");
            List<string> labNames = queueData.LabName.Split(" ").Where(lab => lab != " ").ToList();
            List<Lab> labsToTake = new List<Lab>();

            for(int i = 0; i < labNames.Count(); i++)
            {
                labsToTake.Add(new Lab(){
                    LabName = labNames[i]
                });
            }
            var labEntry = new LabsViewModel
            {
                patient = patientsInQueue,
                Labs = labsToTake
            };
            if (!string.IsNullOrEmpty(labEntry.patient.PatientNo))
            {
                var LabQueue = Queue.GetOrCreateQueue(_context, labEntry.patient.PatientNo, DepartmentType.Lab);
            }
            return View(labEntry);
        }
        return RedirectToAction(nameof(LabQueue));
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
        }else
        {
            patients = patients.OrderBy(q => q.PatientNo)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize);
        }
        

        var model = new PatientListView{
            TotalPatients = _context.Patients.Count(),
            PageSize = pageSize,
            CurrentPage = page,
            Search = search,
            Patients = patients.ToList()
        };

        return View(model);
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
                                p.PatientNo.Contains(search.ToUpper()) ||
                                p.FirstName.Contains(search.Titleize()) ||
                                p.LastName.Contains(search.Titleize()))
                                .OrderBy(q => q.PatientNo)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize);
        }else
        {
            patients = patients.OrderBy(q => q.PatientNo)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize);
        }

        var model = new PatientListView{
            TotalPatients = _context.Patients.Count(),
            PageSize = pageSize,
            CurrentPage = page,
            Search = search,
            Patients = patients.ToList()
        };

        return View(model);
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

        var medicalList = patient.Medicals.Select(m => new MedicalList
        {
            ID = m.ID,
            Date = m.Date,
            Vital = m.Vital,
            Diagnoses = m.Diagnoses,
            Symptom = m.Symptom,
            Drugs = m.Drugs,
            Labs = m.Labs
        }) .OrderByDescending(m => m.Date).ToList();
		
		var Recent = medicalList.OrderByDescending(m => m.Date).FirstOrDefault();
		string mostRecent = "";
		if (Recent != null){
			mostRecent = $"{Recent.Date.Year}-{Recent.Date.Month}-{Recent.Date.Day}";
		}

        var yearlyClass = new Year();
        var monthlyClass = new Month();
        List<Year> yearlyRecords = new List<Year>();
        List<Month> monthlyRecords = new List<Month>();
        List<MedicalList> medicalLists = new List<MedicalList>();
        int prevYear = 0;
		int currentYear = 0;
        int prevMonth = 0;
		int currentMonth = 0;

        // loop through all medical records to generate a list of each year
        foreach (var medical in medicalList)
        {
            currentYear = medical.Date.Year;
			if(currentYear == prevYear){
				continue;
			}
            monthlyRecords = new List<Month>();
            foreach(var month in medicalList)
            {
				medicalLists = new List<MedicalList>();
				currentMonth = month.Date.Month;
				if(currentMonth == prevMonth){
                    continue;
                }
				foreach(var dailyRecord in medicalList)
				{
					if(dailyRecord.Date.Month == currentMonth && dailyRecord.Date.Year == currentYear)
					{
						medicalLists.Add(dailyRecord);
					}
				}
				if(medicalLists.Count() != 0)
				{
					monthlyClass = new Month(){
						MonthNo = currentMonth,
						Date = month.Date,
						MedicalRecords = medicalLists
					};
					monthlyRecords.Add(monthlyClass);
				}
				prevMonth = currentMonth;
            }
            yearlyClass = new Year(){
                YearNo = currentYear,
                Months = monthlyRecords
            };
            yearlyRecords.Add(yearlyClass);
			prevYear = currentYear;
        }

        var viewModel = new PatientMedicalViewModel
        {
            Patient = patient,
            YearlyRecords = yearlyRecords,
            MostRecent = mostRecent
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SavePatientMedicals(CreateMedicalViewModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 3)
        {
            return RedirectToHome();
        }
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
            return RedirectToAction(nameof(DoctorQueue));
        }

        TempData["D_WarningMessage"] = $"Error processing patient's medical details. Please try again";
        return RedirectToAction(nameof(DoctorQueue));
    }


    [HttpGet]
    public IActionResult PharmacyQueue(int page = 1, string search = "")
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 6)
        {
            return RedirectToHome();
        }
        int pageSize = 10;

        var query = _context.Queues.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Pharmacy" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }else{
            query = _context.Queues
                .Include(q => q.Patient)
                .Where(q => q.Status == "Pharmacy")
                .OrderBy(q => q.QueueNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

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
    public async Task<IActionResult> UpdateDrugPrice(PharmacyQueueViewModel model)
    {
        ViewBag.deptId = GetDepartmentId();
        if (ViewBag.deptId != 6)
        {
            return RedirectToHome();
        }
        var dailyData = await _context.DailyDatas.FirstOrDefaultAsync(dd => dd.Date.Date == DateTime.Now.Date);
            if(dailyData == null)
            {
                dailyData = new DailyData()
                {
                    Date = DateTime.Now.Date
                };
                _context.DailyDatas.Add(dailyData);
                await _context.SaveChangesAsync();
            }

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
                var patient = _context.Patients.FirstOrDefault(p => p.PatientNo == PatientNo);
				if(patient.InsuranceExpireDate.Date > DateTime.Now.Date)
                {
                    dailyData.Insurance += bill;
                    _context.DailyDatas.Update(dailyData);
                    await _context.SaveChangesAsync();
                    bill = 0;
                }
				
                medical.Bill += bill;
                dailyData.DrugProfit += bill;

                if(!patient.InsuranceType.ToUpper().Contains("NONE") && patient.InsuranceExpireDate.Date > DateTime.Now.Date && medical.Bill == 0)
                {
                    RemovePatientFromQueue("Pharmacy", PatientNo);
                    await _context.SaveChangesAsync();
                    TempData["P_ConfirmationMessage"] = "Drug prices updated successfully. Insurance was used, patient may leave hospital.";
                    return RedirectToAction("PharmacyQueue");
                }

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
				
				if(bill == 0)
                {
                    TempData["P_ConfirmationMessage"] = "Insurance was used for the drugs, patient may visit cashier to pay remaining bill.";
                    return RedirectToAction("PharmacyQueue");
                }

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
        if (ViewBag.deptId != 7)
        {
            return RedirectToHome();
        }

        int pageSize = 10;
        
        var query = _context.Queues.AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            query = _context.Queues
            .Include(q => q.Patient)
            .Where(q => q.Status == "Cashier" &&
                (q.PatientNo.Contains(search.ToUpper()) ||
                q.Patient.FirstName.Contains(search.Titleize()) ||
                q.Patient.LastName.Contains(search.Titleize())))
            .OrderBy(q => q.QueueNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        }else{
            query = _context.Queues
                .Include(q => q.Patient)
                .Where(q => q.Status == "Cashier")
                .OrderBy(q => q.QueueNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

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
        if (ViewBag.deptId != 7)
        {
            return RedirectToHome();
        }

        if (model.PatientsWithLatestMedical != null)
        {
            var patientWithLatestMedical = model.PatientsWithLatestMedical[0];
            if (patientWithLatestMedical != null)
            {
                string PatientNo = patientWithLatestMedical.PatientQueue.PatientNo;
                var medicalToUpdate = await _context.Medicals.
                    FirstOrDefaultAsync(m => m.ID == patientWithLatestMedical.latestMedical.ID);
                medicalToUpdate.isPaid = true;

                var isDoneQueueNo = GetNextQueueNumber("IsDone");
                var isDoneQueue = new Queue
                {
                    PatientNo = PatientNo,
                    QueueNo = isDoneQueueNo,
                    Status = "IsDone",
                    DateCreated = DateTime.Now
                };
                RemovePatientFromQueue("Cashier", PatientNo);
                _context.Queues.Add(isDoneQueue);
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
