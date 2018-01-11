using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tsr.Core.Models
{
    public class CheckListVM
    {
        public int ApplicationId { get; set; }
        public string Course { get; set; }
        public DateTime? Batchfrom { get; set; }
        public DateTime? Batchto { get; set; }
        public int SrNo { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        public string Cdcno { get; set; }
        public string PassportNo { get; set; }
        public string Rank { get; set; }
        public string Grade { get; set; }
        public string IndosNo { get; set; }
        public string Sign { get; set; }

        public string CompetencyNo { get; set; }

        public string ApplicationCode { get; set; }

        public string App { get; set; }

    }
}
