using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class PackageCourse
    {
        public int PackageCourseId { get; set; }
        public int PackageId { get; set; }
        public int CourseId { get; set; }
    }
}
