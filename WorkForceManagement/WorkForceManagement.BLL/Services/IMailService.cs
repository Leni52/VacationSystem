using System.Threading.Tasks;
using WorkForceManagement.DAL;

namespace WorkForceManagement.BLL.Services
{
    public interface IMailService
    {
        Task SendEmail(MailRequest mailRequest);
    }
}
