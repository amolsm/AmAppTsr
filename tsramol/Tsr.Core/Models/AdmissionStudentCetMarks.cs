using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class AdmissionStudentCetMarks
    {
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public float Marks1 { get; set; }
        public float Marks2 { get; set; }
        public float Marks3 { get; set; }
        public float Marks4 { get; set; }
        public float Total { get; set; }
    }
}
