using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    
    public class ApplicationApplicantsList
    {
        public int? Id { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string PaymentStatus { get; set; }
        public string CourseName { get; set; }
        public string BatchName { get; set; }
        public string PaidAmount { get; set; }
    }
}
