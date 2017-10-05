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
     
       
        public HttpPostedFileBase FileUpload { get; set; }
    }

    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int maxContent = 1024 * 1024; //1 MB
            string[] sAllowedExt = new string[] { ".doc", ".docx"};


            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;
            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = "Please upload Your File of type: " + string.Join(", ", sAllowedExt);
                return false;
            }
            else if (file.ContentLength > maxContent)
            {
                ErrorMessage = "Your File is too large, maximum allowed size is : " + (maxContent / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }
}
