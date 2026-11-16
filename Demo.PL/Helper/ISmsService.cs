using Demo.DAL.Entities;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helper
{
    public interface ISmsService
    {
        MessageResource SendSms(Sms sms);
    }
}
