namespace Persol_HMS.Models.ViewModels
{
    public class DoctorQueueModel
    {
        public List<CreateMedicalViewModel> CreateMedicalViewModel { get; set; }
        public QueueViewModel QueueViewModel { get; set; }
        
        public List<DrugStore> AvailableDrugs { get; set; }
    }
}
