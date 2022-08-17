using ExcelToSQL.Interface;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Mail;

namespace ExcelToSQL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public bool SendEmail(string email, string subject, string message,string fileName,byte[] bytes)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_config.GetSection("ConfirmationParameter:Email").Value);
            mail.Attachments.Add(new Attachment(new MemoryStream(bytes),fileName));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_config.GetSection("ConfirmationParameter:Email").Value, _config.GetSection("ConfirmationParameter:Password").Value);

            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;

                try
                {
                    mail.To.Add(new MailAddress(email));


                    client.Send(mail);

                }
                catch (System.Exception)
                {


                }
                return false;
            }
    }
}
