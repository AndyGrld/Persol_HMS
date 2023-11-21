using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Persol_HMS.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext db;
        public StaffController(ApplicationDbContext _db){
            db = _db;
        }


        // Doctor
        [HttpGet]
        public IActionResult Doctor()
        {
            return View();
        }


        // Records Clerk
        [HttpGet]
        public IActionResult RecordsClerk()
        {
            return View(new Patient());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordsClerk([Bind("FirstName, LastName, DateOfBirth, PatientNo, ContactNo, InsuranceType, InsuranceNo, Gender, EmergencyContactFirstName, EmergencyContactLastName, EmergencyContactNo")] Patient newPatient)
        {
            if(ModelState.IsValid)
            {
                db.Patients.Add(newPatient);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(RecordsClerk));
        }

        [HttpPost]
        public async Task<IActionResult> GetPatient(string patientNo)
        {
            if(patientNo == null)
            {
                var patient = await db.Patients.FirstOrDefaultAsync(p => p.PatientNo == patientNo);
                if (patient != null)
                {
                    return RedirectToAction(nameof(RecordsClerk));
                }
            }
            return RedirectToAction(nameof(RecordsClerk));
        }

        [HttpGet]
        public IActionResult Nurse()
        {
            return View();
        }
    }
}