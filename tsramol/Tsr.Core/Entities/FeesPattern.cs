using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    [Table("FeesPattern")]
    public class FeesPattern
    {
        public int FeesPatternId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }

    
    }
}
