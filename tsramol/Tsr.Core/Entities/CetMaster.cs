using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CetMaster
    {
        public int CetMasterId { get; set; }
        public string CetCode { get; set; }
        public int? CourseId { get; set; }
        //public string CourseName { get; set; }
        public int? BatchId { get; set; }
        //public DateTime? StartDate { get; set; }
        public DateTime? CetDate { get; set; }
        public TimeSpan? CetTime { get; set; }
        public string Venue { get; set; }
        public bool? IsActive { get; set; }

        public string FilePath { get; set; }
    }
}
