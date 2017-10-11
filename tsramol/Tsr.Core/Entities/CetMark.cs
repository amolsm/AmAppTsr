using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class CetMark
    {
        public int CetMarkId { get; set; }
        public int CetMasterId { get; set; }
        public int BatchId { get; set; }
        public int ApplicationId { get; set; }
        public float Marks1 { get; set; }
        public float Marks2 { get; set; }
        public float Marks3 { get; set; }
        public float Marks4 { get; set; }
        public float Total { get; set; }
        public bool? SelectStatus { get; set; }

        public string Hallticketpath { get; set; }

        public bool? IsPublish { get; set; }

      
    }
}
