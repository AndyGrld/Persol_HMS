using Microsoft.EntityFrameworkCore;
using Persol_HMS.Data.Interfaces;

namespace Persol_HMS.Models.Repositories
{
    public class LabRepository : ILabRepository
    {
        private ApplicationDbContext _context;

        public LabRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Lab lab)
        {
            _context.Add(lab);
            return Save();
        }

        public bool Delete(Lab lab)
        {
            _context.Remove(lab);
            return Save();
        }

        public async Task<IEnumerable<Lab>> GetAll()
        {
            return await _context.Labs.ToListAsync();
        }

        public async Task<Lab> GetByIdAsync(int id)
        {
            return await _context.Labs.FirstOrDefaultAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Lab lab)
        {
            _context.Update(lab);
            return Save();
        }


    }
}
