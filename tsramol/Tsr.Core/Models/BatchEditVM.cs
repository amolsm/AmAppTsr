using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class BatchEditVM
    {
        public int BatchId { get; set; }
        [Required]
        [Display(Name = "Batch Code")]
        public string BatchCode { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        [Required]
        [Display(Name = "Course")]
        public int? CourseId { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime? StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime? EndDate { get; set; }
        [Required]
        [Display(Name = "Coordinator")]
        public int? CoordinatorId { get; set; }
        //[Required]
        //[Display(Name = "Designation")]
        //public int? DesignationId { get; set; }
        [Required]
        [Display(Name = "Reserved")]
        public int? ReserveSeats { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [Required]
        [Display(Name = "Online Status")]
        public bool? OnlineBookingStatus { get; set; }


        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CategoryName { get; set; }
        public string CourseName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

     
        public DateTime? CourseExpiryDate { get; set; }
    }
}
