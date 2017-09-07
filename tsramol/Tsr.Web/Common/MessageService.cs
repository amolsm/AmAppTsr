using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);

                return true;
            }
        }
    }

        
}