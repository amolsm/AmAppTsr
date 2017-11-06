using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class CertificateViewResultFormat
    {
        public string OfficeIncharge { get; set; }

        public string BatchCode { get; set; }

        public string CourseName { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? CurrentDate { get; set; }

        public List<CertificateApplicantList> _CertificateApplicantList { get; set; }

    }

    public class CertificateApplicantList
    {
        public int ApplicantId { get; set; }

        public string ApplicantName { get; set; }

        public string Rank { get; set; }

        public string Results { get; set; }

        public string IndosNo  { get; set; }

        public string PassportNo { get; set; }

        public string CdcNo  { get; set; }

        public string CertificateNo    { get; set; }

        public DateTime? DateOfBirth { get; set; }

    }
}
    

