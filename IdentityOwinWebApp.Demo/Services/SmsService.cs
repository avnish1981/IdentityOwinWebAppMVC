using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace IdentityOwinWebApp.Demo.Services
{
    public class SmsService:IIdentityMessageService // This service will allow to send SMS code . For Sending Text message we are using online service called "Trillio"  with free online account .
                                                   //using this API and 
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var sid = ConfigurationManager.AppSettings["twilio:Sid"];
            var token = ConfigurationManager.AppSettings["twilio:Token"];
            var from = ConfigurationManager.AppSettings["twilio:From"];
            TwilioClient.Init(sid, token);
          
          await MessageResource.CreateAsync(new PhoneNumber(message.Destination), from: new PhoneNumber(from), body: message.Body);
          
         
        }
    }
}