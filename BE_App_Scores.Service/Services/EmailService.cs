﻿
using BE_App_Scores.Service.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BE_App_Scores.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig) => _emailConfig = emailConfig;
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }



        /* private MimeMessage CreateEmailMessage(Message message)
         {
             var emailMessage = new MimeMessage();
             emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
             emailMessage.To.AddRange(message.To);
             emailMessage.Subject = message.Subject;
             emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
             return emailMessage;

         }*/


        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            // Aici schimbăm formatul de la Text la HTML
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            return emailMessage;
        }



        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
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
}
