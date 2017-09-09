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

        public string ApplicantName { get; set; }

        public string DateofBirth { get; set; }

        public string CDCNo { get; set; }

        


    }
}
