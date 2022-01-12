using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IUserService
    {
        Task Add(User userToAdd, string password, bool isAdmin);
        Task Delete(Guid userId);
        Task Edit(Guid userId, User editedUser, string editedUserPassword, bool isAdmin);
        Task<List<User>> GetAllUsers();
        List<Team> GetUserTeams(User currentUser);
        Task<User> GetUserById(Guid id);
        Task<User> GetCurrentUser(ClaimsPrincipal principal);
        Task<bool> IsUserAdmin(User currentUser);
        Task MakeUserAdmin(User user);
        Task RemoveUserFromAdmin(User user);
    }
}
