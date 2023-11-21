using Microsoft.EntityFrameworkCore;
using Persol_HMS.Data.Interfaces;
using SQLitePCL;

namespace Persol_HMS.Models.Repositories
{
    public class DugRepository : IDrugRepository
    {
        private ApplicationDbContext _context;

        public DugRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public bool Add(Drug drug)
        {
            _context.Add(drug);
            return Save();
        }

        public bool Delete(Drug drug)
        {
            _context.Remove(drug);
            return Save();
        }

        public async Task<IEnumerable<Drug>> GetAll()
        {
            return await _context.Drugs.ToListAsync();
        }

        public async Task<Drug> GetByIdAsync(int id)
        {
            return await _context.Drugs.FirstOrDefaultAsync( i => i.ID == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Drug drug)
        {
            throw new NotImplementedException();
        }
    }
}
