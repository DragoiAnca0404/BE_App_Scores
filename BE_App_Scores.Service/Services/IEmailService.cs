using BE_App_Scores.Service.Models;

namespace BE_App_Scores.Service.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);

    }
}
