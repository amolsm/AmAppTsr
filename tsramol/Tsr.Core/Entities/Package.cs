using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    [Table("Package")]
    public partial class Package
    {
        public int PackageId { get; set; }

        [Required]
        [StringLength(50)]
        public string PackageName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

      
    }
}
