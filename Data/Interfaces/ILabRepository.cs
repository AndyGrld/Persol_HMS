namespace Persol_HMS.Data.Interfaces
{
    public interface ILabRepository
    {
        Task<IEnumerable<Lab>> GetAll();
        Task<Lab> GetByIdAsync(int id);
        bool Add(Lab lab);
        bool Update(Lab lab);
        bool Delete(Lab lab);
        bool Save();
    }
}
