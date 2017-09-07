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
        public int CD_Id { get; set; }


        public string TopicBeforeCourseTitle { get; set; }
        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }


        public string CourseTitle { get; set; }

        public string Topic1 { get; set; }

        public string Topic2 { get; set; }

        public string Topic3 { get; set; }

        public string Topic4 { get; set; }

        [Required]
        [Display(Name = "Course In Charge")]
        public string CourseIncharge { get; set; }


        [Display(Name = "Principal")]
        public int PrincipalId { get; set; }


    }
    public class CertificateDesignList : CertificateDesignCertificateVM
    {
        public string CourseName { get; set; }
        public string PrincipalName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
