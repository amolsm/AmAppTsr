using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class FeeReceipt
    {
        public int FeeReceiptId { get; set; }
        public string FeeReceiptNo { get; set; }
        public int? ApplicationId { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMode { get; set; }
        public string FeesType { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public bool? PrintStatus { get; set; }

        public string ChequeNo { get; set; }
        public string DDNo { get; set; }
    }
}
