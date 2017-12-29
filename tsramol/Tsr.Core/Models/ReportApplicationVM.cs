using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ReportApplicationVM
    {
      
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int BatchId { get; set; }


        public string revno { get; set; }
        public DateTime? revdate { get; set; }
        public string ApplicationNo { get; set; }

        public int  ApplicationId { get; set; }


        public string NameOfApplicant { get; set; }

        public string Nationality { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string CDCNo { get; set; }
        public string PassportNo { get; set; }
        public string INDOSNo { get; set; }
        public string EnrollNo { get; set; }

        public string CertificateofCompetency { get; set; }

        public string COCNo { get; set; }
        public string ShippingCompany { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string EmailId { get; set; }

        public string Date { get; set; }

        public string CellNo { get; set; }

        public List<ListVarApplicationVM> _ListVar { get; set; }
        public class ListVarApplicationVM
        {


            public string CourseName { get; set; }

            public DateTime? CourseDate { get; set; }

            public string BatchNo { get; set; }
        }
    }

  
}
