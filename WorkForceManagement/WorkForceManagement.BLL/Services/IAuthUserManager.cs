using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IAuthUserManager
    {
        Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);
        Task<bool> IsUserInRole(string userId, string roleId);
        Task AddRoleToUser(User user, string role);
        Task RemoveRoleFromUser(User user, string role);
        Task<User> FindByNameAsync(string name);
        Task<User> FindByIdAsync(string id);
        Task<List<User>> GetAllAsync();
        Task CreateUserAsync(User user, string password);
        Task<List<string>> GetUserRolesAsync(User user);
        Task<bool> ValidateUserCredentials(string userName, string password);
        Task DeleteUserAsync(User user);
        Task<User> FindDifferentUserWithSameUsername(string userId, string username);
        Task EditUserAsync(User user);
    }

}
