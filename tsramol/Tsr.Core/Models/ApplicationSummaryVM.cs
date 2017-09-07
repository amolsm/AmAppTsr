using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ApplicationSummaryVM
    {
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public int? CourseId { get; set; }
        public int? CategoryId { get; set; }
        public int? BatchId { get; set; }
        public decimal? ActualFees { get; set; }
        public decimal? MinBal { get; set; }
    }
}
