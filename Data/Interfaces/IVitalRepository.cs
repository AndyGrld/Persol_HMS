namespace Persol_HMS.Data.Interfaces
{
    public interface IVitalRepository
    {
        Task<IEnumerable<Vital>> GetAll();
        Task<Vital> GetById(int id);
        bool Add(Vital vital);
        bool Update(Vital vital);
        bool Delete(int id);
        bool Save();
    }
}
