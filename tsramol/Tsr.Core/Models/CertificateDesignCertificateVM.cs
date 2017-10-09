using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class CertificateDesignCertificateVM

    {
        public int CertificateDesignId { get; set; }
        [DataType(DataType.MultilineText)]
        public string LineOfCertificate { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [Display(Name = "Course Title")]
        public string CourseName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Paragraph1 { get; set; }
        [DataType(DataType.MultilineText)]
        public string Paragraph2 { get; set; }
        [DataType(DataType.MultilineText)]
        public string Paragraph3 { get; set; }
        [DataType(DataType.MultilineText)]
        public string Topic4 { get; set; }


       


        [Display(Name = "Principal")]
        public int PrincipalId { get; set; }
        [Display(Name = "CertificateFormat")]
        public int CertificateFormatId { get; set; }

        public List<CertificateDesignList> _certificatedesignlist { get; set; }

    }
    public class CertificateDesignList : CertificateDesignCertificateVM
    {
        public string CourseNameTitle { get; set; }
        public string PrincipalName { get; set; }

        public string CertificateFormat { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
