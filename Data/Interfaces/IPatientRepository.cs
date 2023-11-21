namespace Persol_HMS.Data.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAll();
        Task<Patient> GetByIdAsync(string patientNo);
        bool Add(Patient patient);
        bool Update(Patient patient);
        bool Delete(Patient patient);
        bool Save();
    }
}
