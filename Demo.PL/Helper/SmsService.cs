using Demo.DAL.Entities;
using Demo.PL.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helper
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _options;

        public SmsService(IOptions<SmsSettings> options)
        {
            _options = options.Value;
        }
        public MessageResource SendSms(Sms sms)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);

            var result = MessageResource.Create(
                from : new Twilio.Types.PhoneNumber(_options.PhoneNumber),
                body : sms.Body,
                to : sms.PhoneNumber
                );

            return result;
        }
    }
}
