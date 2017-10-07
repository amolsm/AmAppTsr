using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Tsr.Core.Models
{
   public class AddmissionCetCreateVM
    {
        public int? CetId { get; set; }
        [Display(Name ="Cet Code")]
        public string CetCode { get; set; }
        [Display(Name = "Course")]
        public int? CourseId { get; set; }
        //public string CourseName { get; set; }
        [Display(Name = "Batch")]
        public int? BatchId { get; set; }
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "CET Date")]
        public DateTime? CetDate { get; set; }
        [Display(Name = "CET Time")]
        public TimeSpan? CetTime { get; set; }
        public string Venue { get; set; }
        public bool? IsActive { get; set; }
        [Display(Name = "CET Date")]
        public string CetDates { get; set; }
        [Display(Name = "Upload Word file")]

        public List<AddmissionCetListVM> _AddmissionCetListVM { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
    }

   
}
