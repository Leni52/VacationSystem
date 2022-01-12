using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
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

        public async Task Add(User userToAdd, string password, bool isAdmin)
        {
            User foundUser = await _userManager.FindDifferentUserWithSameUsername(Guid.Parse(userToAdd.Id), userToAdd.UserName);
            if (foundUser != null)
            {
                throw new UsernameTakenException($"Username: {userToAdd.UserName} already taken!");
            }

            await _userManager.CreateUser(userToAdd, password);

            if (isAdmin)
            {
                await _userManager.AddRoleToUser(userToAdd, "Admin");
            }
        }

        public async Task Delete(Guid userId)
        {
            User userToDelete = await _userManager.FindById(userId);

            if (userToDelete == null)
                throw new KeyNotFoundException($"User with id: {userId} could not be found! ");

            await _userManager.DeleteUser(userToDelete);
        }

        public async Task Edit(Guid userId, User editedUser, string editedUserPassword, bool isAdmin)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            if (await _userManager.FindDifferentUserWithSameUsername(userId, editedUser.UserName) != null)
            {
                throw new UsernameTakenException($"Username: {editedUser.UserName} already taken!");
            }

            User userToEdit = await _userManager.FindById(userId);

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

            await _userManager.EditUser(userToEdit);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.GetAll();
        }

        public Task<User> GetCurrentUser(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public List<Team> GetUserTeams(User currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserById(Guid id)
        {
            User user = await _userManager.FindById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with id: {id} does not exist!");
            return user;
        }

        public async Task<bool> IsUserAdmin(User currentUser)
        {
            return await _userManager.IsUserInRole(Guid.Parse(currentUser.Id) , "Admin");
        }

        public async Task MakeUserAdmin(User user)
        {
            if (!(await _userManager.IsUserInRole(Guid.Parse(user.Id), "Admin")))
            {
                await _userManager.AddRoleToUser(user, "Admin");
            }
        }

        public async Task RemoveUserFromAdmin(User user)
        {
            if (await _userManager.IsUserInRole(Guid.Parse(user.Id), "Admin"))
            {
                await _userManager.RemoveRoleFromUser(user, "Admin");
            }
        }
    }
}
