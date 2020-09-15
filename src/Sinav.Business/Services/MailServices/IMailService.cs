namespace Sinav.Business.Services.MailServices
{
    public interface IMailService
    {
        void SendEmail();
        void SendConfirmationEmail(string token, string sender, string receiver, string password);
        void SendPasswordResetEmail(string token, string sender, string receiver, string password);
    }
}