using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsr.Core.Models;

namespace Tsr.Core.Entities
{
    [Table("Course")]
   
    public partial class Course
    {
        public int CourseId { get; set; }
        //[Index(IsUnique = true)]
        [StringLength(50)]
        public string CourseCode { get; set; }

        [StringLength(150)]
        [Required]
        //[Index(IsUnique = true)]
        public string CourseName { get; set; }

        public int? CategoryId { get; set; }

        public int? Duration { get; set; }

        [StringLength(10)]
        public string Unit { get; set; }

        public int? TotalSeats { get; set; }

       // public int? Coordinator { get; set; }

       // public int? Designation { get; set; }

        public double? MinAge { get; set; }

        public double? MaxAge { get; set; }

       // public int? FeesPatternId { get; set; }

        //public string Convention { get; set; }

        //[Column(TypeName = "money")]
        //public decimal? ApplicationFee { get; set; }

        //[Column(TypeName = "money")]
        //public decimal? MinBalance { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string ShortName { get; set; }

       
    }
}
