using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CertificateDesign
    {

        public int CertificateDesignId { get; set; }

        public string LineOfCertificate { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string Paragraph1 { get; set; }

        public string Paragraph2 { get; set; }

        public string Paragraph3 { get; set; }

        public string Topic4 { get; set; }


        public int PrincipalId { get; set; }

        private DateTime? createdDate;
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate
        {
            get { return createdDate ?? DateTime.UtcNow; }
            set { createdDate = value; }
        }


    }
}
