namespace Persol_HMS.Data.Interfaces
{
    public interface IMedicalRepository
    {
        Task<IEnumerable<Medical>> GetAll();
        Task<Medical> GetByIdAsync(int id);
        bool Add(Medical medical);
        bool Update(Medical medical);
        bool Delete(Medical medical);
        bool Save();
    }
}
