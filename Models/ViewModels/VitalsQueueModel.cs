using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Models.ViewModels
{
    public class VitalsQueueModel
    {
        public Vital Vital { get; set; }
        public QueueViewModel QueueViewModel { get; set; }
    }
}