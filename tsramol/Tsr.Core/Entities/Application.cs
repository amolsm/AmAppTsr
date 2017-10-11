using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public int? BatchId { get; set; }

        public bool? IsPackage { get; set; } //For PackageCourses
        public int? PackageId { get; set; } //For PackageCourse

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string MotherName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Gender { get; set; }
        public string Citizenship { get; set; }
        public string Caste { get; set; }
        public string Religion { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string Email { get; set; }
        public string CellNo { get; set; }
        public string FatherFullName { get; set; }
        public string FatherOccupation { get; set; }
        public string AnnualIncome { get; set; }
        public string IdentificationMark { get; set; }
        public string PassportNo { get; set; }
        public string ShirtSize { get; set; }
        public string PantSize { get; set; }
        public string ShoeSize { get; set; }
        public string PreferredMeal { get; set; }
        public string PermenentAddress { get; set; }
        public string PermenentCity { get; set; }
        public string PermenentState { get; set; }
        public string PermenentPin { get; set; }
        public string PermenentContactNo { get; set; }
        public string FatherEmail { get; set; }
        public string PresentAddress { get; set; }
        public string PresentCity { get; set; }
        public string PresentState { get; set; }
        public string PresentPin { get; set; }
        public string PresentContactNo { get; set; }
        public string GuardianName { get; set; }
        public string GuardianRelation { get; set; }
        public string GuardianAddress { get; set; }
        public string GuardianCity { get; set; }
        public string GuardianState { get; set; }
        public string GuardianPin { get; set; }
        public string GuardianContact { get; set; }
        public string GuardianEmail { get; set; }
        public string SchoolName { get; set; }
        public string SchoolBoard { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolState { get; set; }
        public string SchoolPin { get; set; }
        public string SchoolPassingYear { get; set; }
        public string SchoolPercentage { get; set; }
        public float? SchoolMath { get; set; }
        public float? SchoolScience { get; set; }
        public float? SchoolEnglish { get; set; }
        public string InterSchoolName { get; set; }
        public string InterBoard { get; set; }
        public string InterAddress { get; set; }
        public string InterCity { get; set; }
        public string InterState { get; set; }
        public string InterPin { get; set; }
        public string InterPassingYear { get; set; }
        public string InterPercentage { get; set; }
        public float? InterMath { get; set; }
        public float? InterPhysics { get; set; }
        public float? InterChemistry { get; set; }
        public float? InterEnglish { get; set; }
        public string InterRollNo { get; set; }
        public string GradCollegeName { get; set; }
        public string GradUniversity { get; set; }
        public string GradAddress { get; set; }
        public string GradCity { get; set; }
        public string GradState { get; set; }
        public string GradPin { get; set; }
        public string GradPassingYear { get; set; }
        public string GradPercentage { get; set; }
        public string GradSubjects { get; set; }
        public bool? GradPassAttempt { get; set; }
        public string CertOfCompetencyNo { get; set; }
        public string CdcNo { get; set; }
        public string InDosNo { get; set; }
        public string GradeOfCompetencyNo { get; set; }
        public string CategoryOfCandidate { get; set; }
        public string ShippingCompany { get; set; }
        public string RankOfCandidate { get; set; }
        public bool? CourseAttendedInTSR { get; set; }
        public string FPFF_AFF_1995 { get; set; }
        public bool? Scrutinee { get; set; }


    }
}
