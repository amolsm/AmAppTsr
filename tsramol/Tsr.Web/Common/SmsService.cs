using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace Tsr.Web.Common
{
    public class SmsService
    {
        public string sendSms(string msg, string mobileno)
        {
            
            String message = HttpUtility.UrlEncode(msg);
            using (var wb = new WebClient())
            {

                byte[] response = wb.UploadValues("http://api.textlocal.in/send/", new NameValueCollection()
                {
                {"username" , "ranjithkumar01@gmail.com"},
                {"hash" , "e399e1b41bbe615c57453488771c9ac83e102d9b87f3b7ae41654d2a8e3c4cb1"},
                {"numbers" , mobileno},
                {"message" , message},
                {"sender" , "TXTLCL"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }
    }
}