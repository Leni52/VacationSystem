using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IFileService
    {
        Task SaveFile(TblFile file);
    }
}
