﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Models
{
    public class EmailModel
    {
        public string From { get; set; }
        public string FromPass { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string File1 { get; set; }

        public string File2 { get; set; }
        //public HttpPostedFileBase Attachment { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
    }
}
