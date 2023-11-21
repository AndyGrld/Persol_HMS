namespace Persol_HMS.Data.Interfaces
{
    public interface IDrugRepository
    {
       Task<IEnumerable<Drug>> GetAll();
        Task<Drug> GetByIdAsync(int id);
        bool Add(Drug drug);
        bool Delete(Drug drug);
        bool Update(Drug drug);
        bool Save();
    }
}
