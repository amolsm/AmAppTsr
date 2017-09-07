using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class Applied
    {
        public int AppliedId { get; set; }
        public int ApplicationId { get; set; }
        public int BatchId { get; set; }
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public bool? AdmissionStatus { get; set; }
    }
}
