using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tsr.Core.Models
{
    public class AdmissionInterviewScheduleVM
    {
        public int InterviewMasterId { get; set; }
        public string InterviewCode { get; set; }
        public int? BatchId { get; set; }
        public int? CourseId { get; set; }
        public DateTime? InterviewDate { get; set; }

        public string Batch { get; set; }

        public string Course { get; set; }
        public string InterviewDates { get; set; }
        public TimeSpan? InterviewTime { get; set; }
        public string Venue { get; set; }

        public HttpPostedFileBase FileUpload { get; set; }

        public List<AdmissionInterviewScheduleVM> _AdmissionInterviewScheduleVM { get; set; }
        
    }

   
}
