using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public  class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string PhoneExtension { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        
        public int DepartmentHead { get; set; }

        [StringLength(50)]
        public string DepHeadCell { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsActive { get; set; }

    
    }
}
