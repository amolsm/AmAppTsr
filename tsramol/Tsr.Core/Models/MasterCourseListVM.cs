using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
   public class MasterCourseListVM
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }

        public string Duration { get; set; }
        public string TotalSeat { get; set; }
        public string Coordinator { get; set; }
        public decimal? ApplicationFee { get; set; }
        public decimal? MinBalance { get; set; }
        public bool? IsActive { get; set; }
    }
}
