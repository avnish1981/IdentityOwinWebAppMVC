using Microsoft.AspNet.Identity;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace IdentityOwinWebApp.Demo.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var client = new SendGridClient(ConfigurationManager.AppSettings["sendgrid:Key"]);
            var from = new EmailAddress("avnish.choubey@gmail.com");
            var ToRecipiants = new EmailAddress(message.Destination);
            var email = MailHelper.CreateSingleEmail(from, ToRecipiants, message.Subject, message.Body, message.Body);
            await client.SendEmailAsync(email);

        }
    }
}