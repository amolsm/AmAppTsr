using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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

        internal static async Task<bool> sendAttachmentEmail(EmailModel obj)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(obj.To));  // replace with valid value 
            message.From = new MailAddress(obj.From);  // replace with valid value
            message.Subject = obj.Subject;
            message.Body = string.Format(obj.Body);
            message.IsBodyHtml = true;
         
            Attachment file1 = new Attachment(obj.File1, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = file1.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(obj.File1);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(obj.File1);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(obj.File1);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(file1);
            Attachment file2 = new Attachment(obj.File2, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition dispositions = file2.ContentDisposition;
            dispositions.CreationDate = System.IO.File.GetCreationTime(obj.File2);
            dispositions.ModificationDate = System.IO.File.GetLastWriteTime(obj.File2);
            dispositions.ReadDate = System.IO.File.GetLastAccessTime(obj.File2);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(file2);


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