using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CertificateNumberNew
    {
        public int CertificateNumberNewId { get; set; }
        public int ApplicationId { get; set; }
        public int BatchId { get; set; }
        public string FullName { get; set; }
        public string CertificateNumber { get; set; }
    }
}
