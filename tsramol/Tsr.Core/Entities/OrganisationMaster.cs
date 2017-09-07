using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    [Table("OrganisationMaster")]
    public class OrganisationMaster
    {
        [Key]
        public int OrganisationId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(50)]
        public string Website { get; set; }

        [StringLength(50)]
        public string TagLine { get; set; }

        [StringLength(50)]
        public string RegisteredNo { get; set; }

        public bool? IsActive { get; set; }
    }
}
