using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Endzone.Umbraco.Extensions.Services
{
    public static class EmailService
    {

        /// <summary>
        /// Send an email using default SMTP settings
        /// </summary>
        /// <param name="fromEmailAddress"></param>
        /// <param name="toEmailAddresses"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="file"> Use Request.Files["filename"] to get file. Form needs to have enctype="multipart/form-data".</param>
        /// <param name="fromName">Display name</param>
        /// <param name="replyTo"></param>
        /// <param name="replyToName">Display name</param>
        public static void SendEmail(string fromEmailAddress, IEnumerable<string> toEmailAddresses, string subject, string body, bool isBodyHtml = false, HttpFileCollection files = null, string fromName = "", string replyTo = "", string replyToName = "")
        {
            using (var smtpClient = new SmtpClient())
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmailAddress, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml,
                    BodyEncoding = Encoding.UTF8,
                    Priority = MailPriority.Normal,
                };
                if (!string.IsNullOrEmpty(replyTo))
                {
                    mail.ReplyToList.Add(new MailAddress(replyTo, !string.IsNullOrEmpty(replyToName) ? "On behalf of " + replyToName : ""));
                }

                foreach (var recipient in toEmailAddresses)
                {
                    mail.To.Add(recipient);
                }

                if (files != null && files.Count > 0)
                {
                    foreach (string key in files)
                    {
                        HttpPostedFile file = files[key];
                        if (file == null || file.ContentLength == 0) continue;
                        string fileName = Path.GetFileName(file.FileName);
                        var attachment = new Attachment(file.InputStream, fileName);
                        mail.Attachments.Add(attachment);
                    }
                }
                smtpClient.Send(mail);
            }
        }
    }

}

