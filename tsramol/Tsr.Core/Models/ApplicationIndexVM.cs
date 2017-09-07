using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class ApplicationIndexVM
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public int CategoryId { get; set; }
        public int CourseId { get; set; }
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        
    }
}
