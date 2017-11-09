using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tsr.Core.Models;

namespace Tsr.Web.Common
{
    public class MessageService
    {

        internal static async Task<bool> sendEmail(EmailModel obj)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(obj.To));  // replace with valid value 
            message.From = new MailAddress(obj.From);  // replace with valid value
            message.Subject = obj.Subject;
            message.Body = string.Format(obj.Body);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = obj.From,  // replace with valid value
                    Password = obj.FromPass  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                //smtp.Host = "smtp.siteprotect.com";
                //smtp.Port = 587;
                smtp.EnableSsl = true;
                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (Exception )
                { }
                return true;
            }
        }
        internal static  bool sendEmail2(EmailModel obj)
        {
            //string MailUser = "onlinebooking@tsrahaman.org";
            //string MailPass = "OB2017tsr";
            //string MailTo = "amolmurkute@gmail.com";
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(obj.To));  // replace with valid value 
                message.From = new MailAddress("onlinebooking@tsrahaman.org");  // replace with valid value
                message.Subject = "Hii";
                message.Body = string.Format(obj.Body);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "onlinebooking@tsrahaman.org",  // replace with valid value
                        Password = "OB2017tsr"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.office365.com";
                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return false;
            }
            //return true;
            }
        
        internal static async Task<bool> sendAttachmentEmail(EmailModel obj)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(obj.To));  // replace with valid value 
            message.From = new MailAddress(obj.From);  // replace with valid value
            message.Subject = obj.Subject;
            message.Body = string.Format(obj.Body);
            message.IsBodyHtml = true;
            if (obj.File1 != null)
            {
                bool exists = File.Exists(obj.File1);
                if (exists == true)
                {
                    System.Net.Mail.Attachment file1 = new System.Net.Mail.Attachment(obj.File1, MediaTypeNames.Application.Octet);
                    // Add time stamp information for the file.
                    ContentDisposition disposition = file1.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(obj.File1);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(obj.File1);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(obj.File1);
                    // Add the file attachment to this e-mail message.
                    message.Attachments.Add(file1);
                }
            }
            if (obj.File2 != null)
            {
                bool exists = File.Exists(obj.File2);
                if (exists == true)
                {
                    System.Net.Mail.Attachment file2 = new System.Net.Mail.Attachment(obj.File2, MediaTypeNames.Application.Octet);
                    // Add time stamp information for the file.
                    ContentDisposition dispositions = file2.ContentDisposition;
                    dispositions.CreationDate = System.IO.File.GetCreationTime(obj.File2);
                    dispositions.ModificationDate = System.IO.File.GetLastWriteTime(obj.File2);
                    dispositions.ReadDate = System.IO.File.GetLastAccessTime(obj.File2);
                    // Add the file attachment to this e-mail message.
                    message.Attachments.Add(file2);
                }
            }

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = obj.From,  // replace with valid value
                    Password = obj.FromPass  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                message.Dispose();
               

                return true;
            }
        }

        public System.Threading.Tasks.Task SendSmsAsync(string msg, string mobileno)
        {
            string message = HttpUtility.UrlEncode(msg);
            string user = ConfigurationManager.AppSettings["SMSUSER"];
            string key = ConfigurationManager.AppSettings["AuthenticationKey"];
            string senderid=ConfigurationManager.AppSettings["SenderId"];
            string accusage=ConfigurationManager.AppSettings["Accusage"];
         

            //Prepare you post parameters
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("user={0}", user);
            sbPostData.AppendFormat("&key={0}", key);
            sbPostData.AppendFormat("&mobile={0}", mobileno);
            sbPostData.AppendFormat("&message={0}", message);
            sbPostData.AppendFormat("&senderid={0}", senderid);
            sbPostData.AppendFormat("&accusage={0}", accusage);

            try
            {
                //Call Send SMS API
                string sendSMSUri = "http://103.233.79.246/submitsms.jsp?";
                //Create HTTPWebrequest
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                //Get the response
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                //Close the response
                reader.Close();
                response.Close();
            }
            catch (SystemException ex)
            {
                string errormsg=ex.Message.ToString();
            }
            //using (var wb = new WebClient())
            //{

            //    byte[] response = wb.UploadValues("http://api.textlocal.in/send/", new NameValueCollection()
            //    {
            //    {"username" , "ranjithkumar01@gmail.com"},
            //    {"hash" , "e399e1b41bbe615c57453488771c9ac83e102d9b87f3b7ae41654d2a8e3c4cb1"},
            //    {"numbers" , mobileno},
            //    {"message" , message},
            //    {"sender" , "TXTLCL"}
            //    });
            //    string result = System.Text.Encoding.UTF8.GetString(response);
            //    //return result;
            //}
            return System.Threading.Tasks.Task.FromResult(0);
        }
    }

        
}