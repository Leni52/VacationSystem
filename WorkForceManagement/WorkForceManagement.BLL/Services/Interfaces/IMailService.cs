using System.Threading.Tasks;
using WorkForceManagement.DAL;

namespace WorkForceManagement.BLL.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmail(MailRequest mailRequest);
    }
}
