using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace movie_hospital_1.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
EnableSsl = true,
UseDefaultCredentials=false,
Credentials=new NetworkCredential("felademerayman@gmail.com", "jcnx yfni rxrr quoz")
            };
            return client.SendMailAsync(
                new MailMessage(from: "felademerayman@gmail.com",
                to: email,
                subject,
                htmlMessage)
                {
                    IsBodyHtml = true
                }
                );
    }
}
    }
