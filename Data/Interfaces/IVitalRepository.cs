namespace Persol_HMS.Data.Interfaces
{
    public interface IVitalRepository
    {
        Task<IEnumerable<Vital>> GetAll();
        Task<Vital> GetByIdAsync(int id);
        bool Add(Vital vital);
        bool Update(Vital vital);
        bool Delete(Vital vital);
        bool Save();
    }
}
