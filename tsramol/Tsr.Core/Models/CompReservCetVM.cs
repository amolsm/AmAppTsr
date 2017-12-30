using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class CompReservCetVM
    {
        public int? CategoryId { get; set; }
        public int? BatchId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CellNo { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Citizenship { get; set; }
        public string Gender { get; set; }
        public string PreferredMeal { get; set; }
        public string CdcNo { get; set; }
        public string PassportNo { get; set; }
        public string InDosNo { get; set; }
        public string GradeOfCompetencyNo { get; set; }
        public string CertOfCompetencyNo { get; set; }
        public string ShippingCompany { get; set; }
        //public bool? CourseAttendedInTSR { get; set; }
        public string PermenentAddress { get; set; }
        public string PermenentCity { get; set; }
        public string PermenentState { get; set; }
        public string PermenentPin { get; set; }
        public string PermenentContactNo { get; set; }
    }
}
