using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IUserService
    {
        Task AddAsync(User userToAdd, string password, bool isAdmin);
        Task DeleteAsync(string userId);
        Task EditAsync(string userId, User editedUser, string editedUserPassword, bool isAdmin);
        Task<List<User>> GetAllUsersAsync();
        List<Team> GetUserTeams(User currentUser);
        Task<User> GetUserWithIdAsync(string id);
        Task<User> GetCurrentUser(ClaimsPrincipal principal);
        Task<bool> IsUserAdmin(User currentUser);
    }
}
