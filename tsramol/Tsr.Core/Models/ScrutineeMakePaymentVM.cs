using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ScrutineeMakePaymentVM
    {
        public decimal? totalFee;
        [Required]
        public decimal? Amount { get; set; }
        public string ApplicationCode { get; set; }
        public int ApplicationId { get; set; }
        [Required]
        public string FeesType { get; set; }
        [Required]
        public string PaymentMode { get; set; }
        public string PaymentLocation { get; set; }
        [Display(Name = "DD No")]
        public string DdNo { get; set; }
        [Display(Name = "Cheque No")]
        public string ChequeNo { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
    }
    public class EditRecieptMpVm
    {
        [Required]
        public decimal? Amount { get; set; }
        public decimal? PrvAmount { get; set; }
        public string ReceiptNo { get; set; }
        //public int ApplicationId { get; set; }
       
        public string FeesType { get; set; }
       
        public string PaymentMode { get; set; }
        public string PaymentLocation { get; set; }
        [Display(Name = "DD No")]
        public string DdNo { get; set; }
        [Display(Name = "Cheque No")]
        public string ChequeNo { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public int? RecieptId { get; set; }
    }
}
