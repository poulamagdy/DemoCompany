using Demo.DAL.Entities;
using Demo.PL.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Demo.PL.Helper
{
	public class EmailSettings : IMailSettings
	{
        //public static void SendEmail(Email email)
        //{
        //	var client = new SmtpClient("smtp.gmail.com", 587);

        //	client.EnableSsl = true;

        //	client.Credentials = new NetworkCredential("poulam574@gmail.com", "kyrhaofbebnxrwhb");

        //	client.Send("poulam574@gmail.com", email.To, email.Subject, email.Body);
        //}
        private readonly MailSettings _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            _options = options.Value;
        }

        public void SendEmail(Email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject
            };

            mail.To.Add(MailboxAddress.Parse(email.To));

            var bulider = new BodyBuilder();

            bulider.TextBody = email.Body;

            mail.Body = bulider.ToMessageBody();

            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            using var smtp = new SmtpClient();

            smtp.Connect(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls);

            smtp.Authenticate(_options.Email, _options.Password);

            smtp.Send(mail);

            smtp.Disconnect(true);
        }
    }
}
