using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
   public class ReportPaymentVM
    {
        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }
       

        public string ApplicantName { get; set; }

        public string CourseOrPackageName  { get; set; }

        public string BatchCode { get; set; }

        public DateTime? BatchStartDate { get; set; }

        public DateTime? BatchEndDate { get; set; }

        public decimal? PaymentAmount { get; set; }

        public string PaymentMode { get; set; }
    }

    public class ReportDatewiseTrainedVM
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int pcnt { get; set; }
        public decimal tax { get; set; }
        public int rcnt { get; set; }
        public string CourseName { get; set; }
        public decimal PackageAmt { get; set; }
        public int PackageCand { get; set; }
        public int RegularCand { get; set; }
        public bool? IsPackage { get; set; }
        public decimal RegularAmt { get; set; }
    }

    public class ReportDatewiseTrainedVM2
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal PackageAmt { get; set; }
        public int PackageCand { get; set; }
        public int RegularCand { get; set; }
        public bool? IsPackage { get; set; }
        public decimal RegularAmt { get; set; }
    }
}
