using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
   public class InterviewMaster
    {
        public int InterviewMasterId { get; set; }
        public string InterviewCode { get; set; }
        public int BatchId { get; set; }
        public DateTime? InterviewDate { get; set; }
        public TimeSpan? InterviewTime { get; set; }
        public string Venue { get; set; }
    }
}
