using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class CertNumberExcelSheetVM
    {
        public int CertificateNumberId { get; set; }
        public int ApplicationId { get; set; }
        public int BatchId { get; set; }
        public string FullName { get; set; }
        public string CertificateNumber { get; set; }
    }
}
