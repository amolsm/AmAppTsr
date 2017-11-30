using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class PaymentStatusSearchVM
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CourseName { get; set; }
        public string Batch { get; set; }
        public string Package { get; set; }
        public string Status { get; set; }
    }
}
