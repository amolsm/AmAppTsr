using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class MedicalMaster
    {
        public int MedicalMasterId { get; set; }
        public string MedicalCode { get; set; }
        public int BatchId { get; set; }

        public int CourseId { get; set; }

        public DateTime? MedicalDate { get; set; }        
        public decimal? MedicalFees { get; set; }

       
    }
}
