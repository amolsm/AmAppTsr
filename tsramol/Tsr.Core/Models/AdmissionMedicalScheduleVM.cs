using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
  public  class AdmissionMedicalScheduleVM
    {
        public int MedicalMasterId { get; set; }
        public string MedicalCode { get; set; }
        [Display(Name = "Batch")]
        public int BatchId { get; set; }
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public DateTime? MedicalDate { get; set; }
        [Display(Name = "MedicalDate")]
        public string MedicalDates { get; set; }
        public decimal? MedicalFees { get; set; }

        public string Batch { get; set; }

        public string Course { get; set; }
    }
}
