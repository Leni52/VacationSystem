using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;

namespace WorkForceManagement.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IRepository<TblFile> _tblFileRepository;

        public FileService(IRepository<TblFile> tblFileRepository)
        {
            _tblFileRepository = tblFileRepository;
        }
        public async Task SaveFile(TblFile file)
        {
            await _tblFileRepository.CreateOrUpdate(file);
        }
    }
}
