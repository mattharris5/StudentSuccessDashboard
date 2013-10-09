using Microsoft.WindowsAzure;
using SSD.Domain;
using SSD.Security.Net;
using System;
using System.Net.Mail;
using System.Net.Mime;

namespace SSD.Security
{
    public class EmailConfirmationManager : IEmailConfirmationManager
    {
        public EmailConfirmationManager(IMailer mailer)
        {
            if (mailer == null)
            {
                throw new ArgumentNullException("mailer");
            }
            Mailer = mailer;
        }

        private IMailer Mailer { get; set; }

        public void Process(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailAddress = user.PendingEmail;
            user.PendingEmail = null;
            user.ConfirmationGuid = Guid.Empty;
        }

        public void Request(User user, Uri confirmationEndpoint)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (confirmationEndpoint == null)
            {
                throw new ArgumentNullException("confirmationEndpoint");
            }
            MailAddress address = new MailAddress(user.PendingEmail, user.DisplayName);
            using (MailMessage message = CreateMessage(address))
            {
                SetupMessageBody(confirmationEndpoint, user.ConfirmationGuid, message);
                Mailer.Send(message);
            }
        }

        private static MailMessage CreateMessage(MailAddress recipient)
        {
            MailMessage message = new MailMessage();
            message.To.Add(recipient);
            message.From = new MailAddress(CloudConfigurationManager.GetSetting("EmailConfirmationFromAddress"), CloudConfigurationManager.GetSetting("EmailConfirmationFromName"));
            message.Subject = CloudConfigurationManager.GetSetting("EmailConfirmationSubject");
            return message;
        }

        private static void SetupMessageBody(Uri confirmationEndpoint, Guid confirmationGuid, MailMessage message)
        {
            string link = confirmationEndpoint.OriginalString + "?identifier=" + confirmationGuid.ToString();
            string text = string.Format(CloudConfigurationManager.GetSetting("EmailConfirmationBodyPlainText"), link);
            string html = string.Format(CloudConfigurationManager.GetSetting("EmailConfirmationBodyHtml"), link);
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
        }
    }
}
