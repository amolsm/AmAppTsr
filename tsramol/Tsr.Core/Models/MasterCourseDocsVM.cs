using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsr.Core.Entities;

namespace Tsr.Core.Models
{
    public class MasterCourseDocsVM
    {
        public int CourseId { get; set; }
        public List<Document> Documents { get; set; }
    }
}
