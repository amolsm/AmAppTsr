using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{

    public class CertificationCertificateVM
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int BatchId { get; set; }
       
        public string PerformAction { get; set; }

        public List<Certificate> _CertificateList   { get; set; }
    
    
        public class Certificate
        {
            public string CertificateNo { get; set; }
            public string ApplicantName { get; set; }

            public DateTime? DateofBirth { get; set; }

            public string CDCNo { get; set; }

            public string PassportNo { get; set; }

            public string Grade { get; set; }

            public string Number { get; set; }

            public string Indosno { get; set; }

            public string LineOfCertificate { get; set; }

            public string CourseName { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
            public string Paragraph1 { get; set; }
            public string Paragraph2 { get; set; }
            public string Paragraph3 { get; set; }

            public string CourseInCharge { get; set; }

            public DateTime? DateOfIssue { get; set; }

            public string DateofExpiry { get; set; }

            public string PrincipalName { get; set; }

           

            public string PrincipalSign { get; set; }

            public string Topic5 { get; set; }

            public string Topic4 { get; set; }









        }

    }

  
}
