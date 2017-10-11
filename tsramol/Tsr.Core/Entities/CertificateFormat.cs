using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CertificateFormat
    {
        public int CertificateFormatId { get; set; }
        public string FormatName { get; set; }

        public string ActionName { get; set; }

        public bool? IsActive { get; set; }

      
    }
}
