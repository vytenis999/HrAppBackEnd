using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MouseTagProject.Context;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using System.Text;

namespace ToDoListProject.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailconfig;
        private readonly MouseTagProjectContext _context;

        public EmailSender(EmailConfiguration emailconfig, MouseTagProjectContext context)
        {
            _emailconfig = emailconfig;
            _context = context;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailconfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailconfig.SmtpServer, _emailconfig.Port, true);
                    client.Authenticate(_emailconfig.UserName, _emailconfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public string GenerateLetter()
        {
            string table = Encoding.UTF8.GetString(File.ReadAllBytes($"/home/site/wwwroot/Templates/EmailTemplate.html"));
            string candidateLine = "";
            string clientLine = "";
            var candidates = _context.Candidates.Where(c => c.WillBeContacted != null && c.WillBeContacted > DateTime.Now && c.WillBeContacted <= DateTime.Now.AddDays(7)).ToList();
            var clients = _context.Clients.Where(c => c.WillBeContacted != null && c.WillBeContacted > DateTime.Now && c.WillBeContacted <= DateTime.Now.AddDays(7)).ToList();
            foreach (var candidate in candidates)
            {
                string trBlank = Encoding.UTF8.GetString(File.ReadAllBytes($"/home/site/wwwroot/Templates/TrElement.html"));
                string tr = trBlank.Replace("{1}", candidate.Name).Replace("{2}", candidate.Surname).Replace("{3}", candidate.Comment).Replace("{4}", candidate.Linkedin).Replace("{5}", candidate.WillBeContacted.Value.ToString("yyyy-MM-dd"));
                candidateLine += tr;
            }

            foreach (var client in clients)
            {
                string trBlank = Encoding.UTF8.GetString(File.ReadAllBytes($"/home/site/wwwroot/Templates/ClientTrElement.html"));
                string tr = trBlank.Replace("{1}", client.Name).Replace("{2}", client.Project).Replace("{3}", client.Comment).Replace("{4}", client.WillBeContacted.Value.ToString("yyyy-MM-dd"));
                clientLine += tr;
            }

            return table.Replace("{1}", candidateLine).Replace("{2}", clientLine);
        }
    }
}

