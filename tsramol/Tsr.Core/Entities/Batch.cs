using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class Batch
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CoordinatorId { get; set; }
        //public int? DesignationId { get; set; }
        public int? ReserveSeats { get; set; }
        public int? BookedSeats { get; set; }
        public int? TotalSeats { get; set; }
        public bool? IsActive { get; set; }
        public bool? OnlineBookingStatus { get; set; }


        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
      
      public string Remark { get; set; }

    }
}
