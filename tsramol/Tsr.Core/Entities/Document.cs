using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    [Table("Document")]
    public class Document
    {
        [Key]
        public int DocumentsListId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentName { get; set; }

        [StringLength(50)]
        public string DocumentType { get; set; }

        public bool? IsActive { get; set; }

        
    }
}
