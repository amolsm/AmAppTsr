using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ApplicationSumPayCetVM
    {
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CellNo { get; set; }
        public string CourseName { get; set; }
        public string BatchCode { get; set; }

        //public decimal? CourseFee { get; set; }
        public int? CourseId { get; set; }
        public int? CategoryId { get; set; }
        public int? BatchId { get; set; }
        //public decimal? TaxAmount { get; set; }
        public string key { get; set; }
        public string txnid { get; set; }
        public string hash { get; set; }
        public decimal? amount { get; set; }
        
        public string curl { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
    }
}
