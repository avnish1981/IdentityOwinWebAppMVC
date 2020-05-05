using Microsoft.AspNet.Identity;
using SendGrid;
using System.Threading.Tasks;
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
            var ToRecipiants = new EmailAddress("avnish.choubey@gmail.com");
            var email = MailHelper.CreateSingleEmail(from, ToRecipiants, message.Subject, message.Body, message.Body);
            await client.SendEmailAsync(email);

        }
    }
}