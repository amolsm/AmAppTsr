using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class AdmissionConfirmDGExcelFormat
    {
        public int? SrNo { get; set; }
        public string CandidateName { get; set; }
        public string DateOfBirth { get; set; }
        public string INDoS { get; set; }
        public string PassportNo { get; set; }
        public string CDC_No { get; set; }
        public string DateOfJoining { get; set; }
        public string Remarks { get; set; }
    }
}
