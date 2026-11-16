using Demo.DAL.Entities;

namespace Demo.PL.Helper
{
    public interface IMailSettings
    {
        void SendEmail(Email email);
    }
}
