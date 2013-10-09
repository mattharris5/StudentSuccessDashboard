using Microsoft.WindowsAzure;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace SSD.Security.Net
{
    public class Mailer : IMailer
    {
        private string SmtpHost
        {
            get { return CloudConfigurationManager.GetSetting("SmtpHost"); }
        }
        private int SmtpPort
        {
            get { return int.Parse(CloudConfigurationManager.GetSetting("SmtpPort")); }
        }
        private string SmtpUserName
        {
            get { return CloudConfigurationManager.GetSetting("SmtpUserName"); }
        }
        private string SmtpPassword
        {
            get { return CloudConfigurationManager.GetSetting("SmtpPassword"); }
        }

        public void Send(MailMessage message)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(SmtpHost, SmtpPort))
                {
                    NetworkCredential credentials = new NetworkCredential(SmtpUserName, SmtpPassword);
                    smtpClient.Credentials = credentials;
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
