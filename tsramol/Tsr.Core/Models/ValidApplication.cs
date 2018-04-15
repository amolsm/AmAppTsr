using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
   public class ValidApplication
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CourseId { get; set; }
        public int? BatchId { get; set; }
        public string CellNo { get; set; }
        public int PackageId { get; set; }
    }
}
