using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface IEmailSender
    {
        string GenerateLetter();
        void SendEmail(Message message);
    }
}
