using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
   public class CourseFeeListVM
    {
        public int CourseFeeId { get; set; }
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public decimal? CourseFee { get; set; }
        public decimal? PackageFee { get; set; }
    }
}
