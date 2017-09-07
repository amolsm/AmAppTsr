using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class MastersCourseFeeCreateVM
    {
        public int? CourseFeesId { get; set; }
        [Required]
        [Display(Name ="Category")]
        public int? CategoryId { get; set; }
        [Required]
        [Display(Name = "Course")]
        public int? CourseId { get; set; }
        [Required]
        [Display(Name = "Fees Pattern")]
        public int? FeesPatternId { get; set; }
        [Required]
        [Display(Name = "Actual Fee")]
        public decimal? ActualFee { get; set; }
        [Required]
        [Display(Name = "Package Fee")]
        public decimal? PackageFee { get; set; }
        [Required]
        [Display(Name = "Application Fee")]
        public decimal? ApplicationFee { get; set; }
        [Required]
        [Display(Name = "Min Balance")]
        public decimal? MinBalance { get; set; }
        [Required]
        [Display(Name = "GST %")]
        public double? GstPercentage { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
