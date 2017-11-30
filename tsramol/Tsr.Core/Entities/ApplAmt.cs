using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class ApplAmt
    {
        public int ApplAmtId { get; set; }
        public int? ApplicationId { get; set; }
        public decimal? Amount { get; set; }
    }
}
