using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class AdmissionConfirmListVM
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public bool Select { get; set; }
        public string FeePaid { get; set; }

        public string Batch { get; set; }

        public string Course { get; set; }
    }
}
