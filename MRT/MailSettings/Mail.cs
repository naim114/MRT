using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MVC1006.MailSettings
{
    public class Mail {
        private IConfiguration configuration;
        public Mail(IConfiguration config) { 
            configuration = config; 
        } 
        
        // Method to send mail
        public bool Send(string from, string to, string subject, string body) { 
            try { 
                // Retrieving mail settings from appsettings.json
                var host = configuration["Gmail:Host"];
                var port = int.Parse(configuration["Gmail:Port"]);
                var username = configuration["Gmail:Username"];
                var password = configuration["Gmail:Password"];
                var enable = bool.Parse(configuration["Gmail:SMTP:starttls:enable"]);
                var smtpClient = new SmtpClient {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                }; 
                var mailMessage = new MailMessage(from, to);                 
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
                
                return true;             
            } catch { 
                return false;
            }         
        }     
    }
}
