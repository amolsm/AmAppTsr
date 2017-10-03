using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class MasterPackageListVM
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public bool IsActive { get; set; }
        public List<string> Courses { get; set; }
    }
}
