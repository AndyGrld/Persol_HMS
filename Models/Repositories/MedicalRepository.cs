using Microsoft.EntityFrameworkCore;
using Persol_HMS.Data.Interfaces;

namespace Persol_HMS.Models.Repositories
{
    public class MedicalRepository : IMedicalRepository
    {
        private ApplicationDbContext _context;

        public MedicalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Medical medical)
        {
            _context.Add(medical);
            return Save();
        }

        public bool Delete(Medical medical)
        {
            _context.Remove(medical);
            return Save();
        }

        public async Task<IEnumerable<Medical>> GetAll()
        {
            return await _context.Medicals.ToListAsync();
        }

        public async Task<Medical> GetByIdAsync(int id)
        {
            return await _context.Medicals.FirstOrDefaultAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Medical medical)
        {
            _context.Update(medical);
            return Save();
        }
    }
}
