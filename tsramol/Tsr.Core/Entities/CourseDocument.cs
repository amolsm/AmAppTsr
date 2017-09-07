using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CourseDocument
    {
        public int CourseDocumentId { get; set; }
        public int CourseId { get; set; }
        public int DocumentId { get; set; }
    }
}
