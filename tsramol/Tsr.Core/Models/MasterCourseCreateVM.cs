using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TugberkUg.MVC.Validation;

namespace Tsr.Core.Models
{
    public class MasterCourseCreateVM
    {
        public int CourseId { get; set; }

        [StringLength(50)]
        [Required]
        public string CourseCode { get; set; }

        [StringLength(150)]
        [Required]
        public string CourseName { get; set; }
        public string ShortName { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public int? Duration { get; set; }

        [StringLength(10)]
        [Required]
        public string Unit { get; set; }
        [Required]
        public int? TotalSeats { get; set; }
        //[Required]
        //public int? Coordinator { get; set; }
        //[Required]
        //public int? Designation { get; set; }

        public double? MinAge { get; set; }

        public double? MaxAge { get; set; }

        //public int? FeesPatternId { get; set; }

        //public string Convention { get; set; }

       
        //public decimal? ApplicationFee { get; set; }

        
        //public decimal? MinBalance { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Display(Name ="CourseExpiry")]
        public DateTime? CourseExpiryDate { get; set; }
    }
   
   

    
}
