﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class HallTicketListVM
    {
        public int? Id { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string PaymentStatus { get; set; }
        public string CourseName { get; set; }
        public string BatchName { get; set; }
        public DateTime? CetDate { get; set; }
        public TimeSpan CetTime { get; set; }
        public string PaidAmount { get; set; }
        public string Fathername { get; set; }
        public string Mothername { get; set; }
        public int CetMasterId { get; set; }
        public bool Select { get; set; }
        public string Dob { get; set; }
        public int BatchId { get; set; }
    }

    public class HallTicketListEx
    {
        //public int? Id { get; set; }
        //public int? ApplicationId { get; set; }
        public string ApplicationCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        //public string PaymentStatus { get; set; }
        public string CourseName { get; set; }
        public string Batch { get; set; }
        public DateTime? CetDate { get; set; }
        public TimeSpan CetTime { get; set; }
        //public string PaidAmount { get; set; }
       // public string Fathername { get; set; }
       // public string Mothername { get; set; }
        //public int CetMasterId { get; set; }
        //public bool Select { get; set; }

        //public int BatchId { get; set; }
    }
}
