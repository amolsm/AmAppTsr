using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class BatchListVM
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
       // public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CoordinatorId { get; set; }
        public int? DesignationId { get; set; }
        public int? ReserveSeats { get; set; }

        public bool? IsActive { get; set; }
        public bool? OnlineBookingStatus { get; set; }
    }
}
