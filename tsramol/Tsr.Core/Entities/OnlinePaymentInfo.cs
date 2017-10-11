using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class OnlinePaymentInfo
    {
        public int OnlinePaymentInfoId { get; set; }
        public int? ApplicationId { get; set; }
        public int? BatchId { get; set; }
        public int? CourseId { get; set; }
        public int? CategoryId { get; set; }
        public int? PackageId { get; set; }
        public bool? IsPackage { get; set; }

        public string mihpayid { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public string key { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string Productinfo { get; set; }        
        public string Hash { get; set; }
        public string bank_ref_num { get; set; }
        public DateTime? PaymentDate { get; set; }

        public string udf1 { get; set; } //BatchId
        public string udf2 { get; set; } //ApplicationCode
        public string udf3 { get; set; } //ApplicationId
        public string udf4 { get; set; } //
        public string udf5 { get; set; } //

        
    }
}
