using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class AdmissionViewApplicantsVM
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicantName { get; set; }
        public string CourseName { get; set; }
        public string BatchCode { get; set; }
        public string CategoryName { get; set; }
    }
}
