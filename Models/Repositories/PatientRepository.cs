using Microsoft.EntityFrameworkCore;

namespace Persol_HMS.Models.Repositories
{
    public class PatientRepository
    {
        private ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Patient patient)
        {
            _context.Add(patient);
            return Save();
        }

        public bool Delete(Patient patient)
        {
            _context.Remove(patient);
            return Save();
        }

        public async Task<IEnumerable<Patient>> GetAll()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(string patientNo)
        {
            return  await _context.Patients.FirstOrDefaultAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Patient patient)
        {
            _context.Update(patient);
            return Save();
        }
    }
}
