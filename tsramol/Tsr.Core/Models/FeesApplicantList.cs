using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class FeesApplicantList
    {
        public int? BatchId { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string PaymentMode { get; set; }
        public string FeesType { get; set; }
        public string BatchCode { get; set; }
        public string PaidAmount { get; set; }
        public string BalanceAmount { get; set; }
        public string FeeReceiptNo { get; set; }
        public bool? Flag { get; set; }
    }
}
