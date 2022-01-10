using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthUserManager _userManager;

        public UserService(IAuthUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task AddAsync(User userToAdd, string password, bool isAdmin)
        {
            if(await _userManager.FindDifferentUserWithSameUsername(userToAdd.Id, userToAdd.UserName) != null)
            {
                return;
            }

            await _userManager.CreateUserAsync(userToAdd, password);

            if (isAdmin)
            {
                await _userManager.AddRoleToUser(userToAdd, "Admin");
            }
        }

        public async Task DeleteAsync(string userId)
        {
            User userToDelete = await _userManager.FindByIdAsync(userId);

            if (userToDelete == null)
                throw new KeyNotFoundException($"User with id: {userId} could not be found! ");

            await _userManager.DeleteUserAsync(userToDelete);
        }

        public async Task EditAsync(string userId, User editedUser, string editedUserPassword, bool isAdmin)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            if (await _userManager.FindDifferentUserWithSameUsername(userId, editedUser.UserName) != null)
            {
                return;
            }

            User userToEdit = await _userManager.FindByIdAsync(userId);

            if (userToEdit == null)
                throw new KeyNotFoundException($"User with Id:{userId} was not found");

            userToEdit.UserName = editedUser.UserName;
            userToEdit.PasswordHash = hasher.HashPassword(editedUser, editedUserPassword);

            if (isAdmin && !(await _userManager.IsUserInRole(userId, "Admin")))
            {
                await _userManager.AddRoleToUser(userToEdit, "Admin");
            }
            else if(!isAdmin && await _userManager.IsUserInRole(userId, "Admin"))
            {
                await _userManager.RemoveRoleFromUser(userToEdit, "Admin");
            }

            await _userManager.EditUserAsync(userToEdit);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.GetAllAsync();
        }

        public Task<User> GetCurrentUser(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public List<Team> GetUserTeams(User currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserWithIdAsync(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with id: {id} does not exist!");
            return user;
        }

        public async Task<bool> IsUserAdmin(User currentUser)
        {
            return await _userManager.IsUserInRole(currentUser.Id , "Admin");
        }
    }
}
