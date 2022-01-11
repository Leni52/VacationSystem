using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface IAuthUserManager
    {
        Task<bool> IsUserInRole(Guid userId, string roleId);
        Task AddRoleToUser(User user, string role);
        Task RemoveRoleFromUser(User user, string role);
        Task<User> FindById(Guid id);
        Task<List<User>> GetAll();
        Task CreateUser(User user, string password);
        Task<List<string>> GetUserRoles(User user);
        Task<bool> ValidateUserCredentials(string userName, string password);
        Task DeleteUser(User user);
        Task<User> FindDifferentUserWithSameUsername(Guid userId, string username);
        Task EditUser(User user);
        Task<User> FindByName(string userName);
    }

}
