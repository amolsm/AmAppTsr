using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ScrutineeMakePaymentVM
    {
        public decimal? totalFee;

        public decimal? Amount { get; set; }
        public string ApplicationCode { get; set; }
        public int ApplicationId { get; set; }
        public string FeesType { get; set; }
        public string PaymentMode { get; set; }
    }
}
