using System.Net.Mail;

namespace SSD.Security.Net
{
    public interface IMailer
    {
        void Send(MailMessage message);
    }
}
