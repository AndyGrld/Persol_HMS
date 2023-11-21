using Microsoft.EntityFrameworkCore;

namespace Persol_HMS.Models.Repositories
{
    public class LabRepository
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
            return await _context.Labs.FirstOrDefaultAsync(i => i.ID == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Lab lab)
        {
            throw new NotImplementedException();
        }
    }
}
