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
        // Doctor
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Doctor(string username)
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
                _context.Patients.Add(newPatient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(RecordsClerk));
        }

        [HttpPost]
        public async Task<IActionResult> GetPatient(string patientNo)
        {
            if(patientNo == null)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientNo == patientNo);
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