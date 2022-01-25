using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IUserService
    {
        Task Add(User user, string password, bool isAdmin);
        Task Delete(Guid id);
        Task Update(User user, string oldEmail, string newPassword, bool isAdmin);
        Task<List<User>> GetAllUsers();
        List<Team> GetUserTeams(User user);
        Task<User> GetUserById(Guid id);
        Task<User> GetCurrentUser(ClaimsPrincipal principal);
        Task<bool> IsUserAdmin(User user);
        Task MakeUserAdmin(Guid id);
        Task RemoveUserFromAdmin(Guid id);
        Task<List<User>> GetUsersUnderTeamLeader(User user);
        Task ConfirmEmailAdress(string userId, string token);
    }
}
