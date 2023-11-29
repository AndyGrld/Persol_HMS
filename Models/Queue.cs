using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persol_HMS.Models
{
    public class Queue
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Patient))]
        public string PatientNo { get; set; }

        public int QueueNo { get; set; }
        public string? LabName { get; set; }

        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual Patient Patient { get; set; }

        public static Queue GetOrCreateQueue(ApplicationDbContext context, string patientNo, DepartmentType department)
        {
            DateTime currentDate = DateTime.Now.Date;

            var existingQueue = context.Queues
                .FirstOrDefault(q => q.PatientNo == patientNo &&
                                    q.DateCreated.Date == currentDate &&
                                    q.Status == department.ToString());

            if (existingQueue != null)
            {
                return existingQueue;
            }

            // If no queue exists, create a new one
            var newQueue = new Queue
            {
                PatientNo = patientNo,
                QueueNo = GetNextQueueNumber(context, department),
                Status = department.ToString(),
                DateCreated = currentDate
            };

            context.Queues.Add(newQueue);
            context.SaveChanges();

            return newQueue;
        }

        private static int GetNextQueueNumber(ApplicationDbContext context, DepartmentType department)
        {
            // Get the maximum queue number for the specified department and date
            var maxQueueNumber = context.Queues
                .Where(q => q.Status == department.ToString() && q.DateCreated.Date == DateTime.Now.Date)
                .Max(q => (int?)q.QueueNo) ?? 0;

            return maxQueueNumber + 1;
        }
    }
}
