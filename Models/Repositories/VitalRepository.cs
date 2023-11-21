using Microsoft.EntityFrameworkCore;

namespace Persol_HMS.Models.Repositories
{
    public class VitalRepository
    {
        private ApplicationDbContext _context;

        public VitalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Vital vital)
        {
            _context.Add(vital);
            return Save();
        }

        public bool Delete(Vital vital)
        {
            _context.Remove(vital);
            return Save();
        }

        public async Task<IEnumerable<Vital>> GetAll()
        {
            return await _context.Vitals.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(string patientNo)
        {
            return await _context.Patients.FirstOrDefaultAsync(i => i.PatientNo == patientNo);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Vital vital)
        {
            throw new NotImplementedException();
        }
    }
}
