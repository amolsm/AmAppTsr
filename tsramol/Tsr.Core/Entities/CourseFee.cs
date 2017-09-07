using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CourseFee
    {
        public int CourseFeeId { get; set; }
        
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public int? FeesPatternId { get; set; }
        [Column(TypeName = "money")]
        public decimal? ActualFee { get; set; }
        [Column(TypeName = "money")]
        public decimal? PackageFee { get; set; }
        [Column(TypeName = "money")]
        public decimal? ApplicationFee { get; set; }
        public decimal? MinBalance{ get; set; }
        public double? GstPercentage { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
