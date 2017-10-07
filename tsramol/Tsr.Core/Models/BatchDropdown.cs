using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class BatchDropdown
    {
        public string BatchCode { get; set; }
        public int BatchId { get; set; }
        public string Name { get; set; }
    }
    public class CetDropdown
    {
        public string CetCode { get; set; }
        public int CetMasterId { get; set; }
        public string Name { get; set; }
    }
    public class InterviewDropdown
    {
        public string InterviewCode { get; set; }
        public int InterviewMasterId { get; set; }
        public string Name { get; set; }
    }
    public class MedicalDropdown
    {
        public string MedicalCode { get; set; }
        public int MedicalMasterId { get; set; }
        public string Name { get; set; }
    }
}
