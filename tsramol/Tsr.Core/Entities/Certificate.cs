using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class Certificate
    {
        public int CertificateId { get; set; }

        public string CertificateCode { get; set; }

        public int ApplicationId { get; set; }

        public int BatchId { get; set; }
        public DateTime? CreateDate { get; set; }

        public bool IsPrint { get; set; }
    }
}
