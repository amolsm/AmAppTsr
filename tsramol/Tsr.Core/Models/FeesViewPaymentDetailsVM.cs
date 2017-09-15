using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class FeesViewPaymentDetailsVM
    {

        public int FeeReceiptId { get; set; }
        public string FeeReceiptNo { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMode { get; set; }
        public string FeesType { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public bool? PrintStatus { get; set; }

        public string ChequeNo { get; set; }
        public string DDNo { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public string Batch { get; set; }
        public int StudentId { get; set; }
        public decimal? FeePaid { get; set; }
        public decimal? FeeBal { get; set; }
        public DateTime? BatchStartDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public string AmountInRs { get; set; }
    }
}
