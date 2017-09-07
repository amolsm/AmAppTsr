using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ApplicationPaymentSuccess
    {
        public int Id { get; set; }
        public string mihpayid { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public string key { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string Productinfo { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Hash { get; set; }
        public string bank_ref_num { get; set; }

        public string udf1 { get; set; } //BatchId
        public string udf2 { get; set; } //ApplicationCode
        public string udf3 { get; set; } //
        public string udf4 { get; set; } //
        public string udf5 { get; set; } //

        //NonResponse Variables
        public string CourseName { get; set; }


    }
}
