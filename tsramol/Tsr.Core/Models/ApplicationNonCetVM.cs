using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ApplicationNonCetVM
    {
        public List<PackageCourseBatches> PackageBatchId { get; set; } //only for Packages
        public int? PackageId { get; set; } //for Packages
        public int? Id { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public int? CategoryId { get; set; }
        public int? BatchId { get; set; }
        public string ApplicationCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Gender { get; set; }
        public string Citizenship { get; set; }
        public string Email { get; set; }
        public string CellNo { get; set; }
        public string PreferredMeal { get; set; }
        public string CdcNo { get; set; }
        public string PassportNo { get; set; }
        public string InDosNo { get; set; }
        public string CertOfCompetencyNo { get; set; }
        public string GradeOfCompetencyNo { get; set; }
        public string CategoryOfCandidate { get; set; }
        public string ShippingCompany { get; set; }
        public string RankOfCandidate { get; set; }
        public bool? CourseAttendedInTSR { get; set; }
        
        public string FPFF_AFF_1995 { get; set; } //for refresher only

    }
}
