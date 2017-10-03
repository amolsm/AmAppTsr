using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class AddmissionCetListVM
    {
        public int? CetId { get; set; }
        public string CetCode { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string BatchCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CetDate { get; set; }
        public TimeSpan? CetTime { get; set; }
        public string Venue { get; set; }
        public bool? IsActive { get; set; }

        public int CourseId { get; set; }
    }
}
