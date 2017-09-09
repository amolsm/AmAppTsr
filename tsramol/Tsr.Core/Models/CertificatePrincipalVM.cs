using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tsr.Core.Entities;

namespace Tsr.Core.Models
{
   public class CertificatePrincipalVM
    {
        public int PrincipalId { get; set; }
        [Required]
        public string PricipalName { get; set; }

        [Display(Name = "Signature")]
        public string SignatureImgUrl { get; set; }

       

        public List<Principal> _principalList { get; set; }


    }
    
}
