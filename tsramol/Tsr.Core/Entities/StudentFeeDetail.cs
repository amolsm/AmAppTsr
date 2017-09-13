using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class StudentFeeDetail
    {
        public int StudentFeeDetailId { get; set; }
        public int? ApplicationId { get; set; }
        public int? PackageId { get; set; }
        public int? BatchId { get; set; }
        public decimal? TotalFee { get; set; }
        public decimal? FeePaid { get; set; }
        public decimal? FeeBal { get; set; }
    }
}
