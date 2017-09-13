using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class ApplicationPackageDetail
    {
        public int ApplicationPackageDetailId { get; set; }
        public int ApplicationId { get; set; }
        public int PackageId { get; set; }
        public int CourseId { get; set; }
        public int BatchId { get; set; }
        public bool? ConfirmStatus { get; set; }
    }
}
