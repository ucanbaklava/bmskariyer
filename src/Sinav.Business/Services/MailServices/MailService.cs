using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Sinav.Business.Services.MailServices
{
    public class MailService: IMailService
    {
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }
        public void SendEmail()
        {
            throw new System.NotImplementedException();
        }

        public void SendConfirmationEmail(string token, string sender, string receiver, string password)
        {
            var client = new SmtpClient("smtp.yandex.com.tr", 587);
            var mail = new MailMessage();
            mail.From = new MailAddress(sender, "BMS KARİYER"); //gönderici olarak görünen mail bilgileri
            mail.Priority = MailPriority.High;
            mail.Subject = "BMS Kariyer Üyelik Onaylama Maili";
            mail.Body = token;
            mail.IsBodyHtml = true;
            mail.To.Add(new MailAddress(receiver));

            var credentials = new NetworkCredential(sender, password);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                client.Send(mail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Onay Maili Gönderirken HATA");

            }


        }
        
        public void SendPasswordResetEmail(string token, string sender, string receiver, string password)
        {
            var client = new SmtpClient("smtp.yandex.com.tr", 587);
            var mail = new MailMessage();
            mail.From = new MailAddress(sender, "BMS KARİYER"); //gönderici olarak görünen mail bilgileri
            mail.Priority = MailPriority.High;
            mail.Subject = "BMS Kariyer Şifre Sıfırlama";
            mail.Body = token;
            mail.IsBodyHtml = true;
            mail.To.Add(new MailAddress(receiver));

            var credentials = new NetworkCredential(sender, password);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                client.Send(mail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Şifre Sırıfırlama Maili Gönderirken HATA");

            }


        }
    }
}