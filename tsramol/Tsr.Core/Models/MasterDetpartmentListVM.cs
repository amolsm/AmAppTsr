using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class MasterDetpartmentListVM
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Address { get; set; }
        public string Extension { get; set; }
        public string Phone { get; set; }
        public string DepartmentHead { get; set; }
        public string Cellphone { get; set; }
        public bool? IsActive { get; set; }
    }
}
