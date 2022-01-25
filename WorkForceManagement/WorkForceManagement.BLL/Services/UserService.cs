using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.DAL;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ITeamService _teamService;
        private readonly IAuthUserManager _userManager;
        private readonly IMailService _mailService;

        public UserService(ITeamService teamService, IAuthUserManager userManager, IMailService mailService)
        {
            _teamService = teamService;
            _userManager = userManager;
            _mailService = mailService;
        }

        public async Task Add(User userToAdd, string password, bool isAdmin)
        {
            User foundUser = await _userManager.FindDifferentUserWithSameUsername(Guid.Parse(userToAdd.Id), userToAdd.UserName);
            if (foundUser != null)
            {
                throw new UsernameTakenException($"Username: {userToAdd.UserName} already taken!");
            }

            await IsEmailValid(userToAdd.Email);

            userToAdd.TwoFactorEnabled = true;
            await _userManager.CreateUser(userToAdd, password);

            await EmailUserWithConfirmationToken(userToAdd);

            if (isAdmin)
            {
                await _userManager.AddRoleToUser(userToAdd, "Admin");
            }
        }

        private async Task EmailUserWithConfirmationToken(User userToAdd)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(userToAdd);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string appUrl = "https://localhost:5000"; // TODO this probably should be changed to be included in app settings
            string url = $"{appUrl}/api/User/confirmemail?userid={userToAdd.Id}&token={validEmailToken}";

            await _mailService.SendEmail(new MailRequest()
            {
                ToEmail = userToAdd.Email,
                Subject = "Confirm your email",
                Body = $"<p>Confirm your email by <a href='{url}'> Clicking Here </a>  </p>"
            });
        }

        private async Task IsEmailValid(string emailAddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailAddress);

                if (await _userManager.FindByEmail(emailAddress) != null)
                {
                    throw new EmailAddressAlreadyInUseException($"Email: {emailAddress} already in use!");
                }
            }
            catch (FormatException)
            {
                throw new InvalidEmailException($"Email: {emailAddress} is not valid!");
            }
        }

        public async Task Delete(Guid userId)
        {
            User userToDelete = await _userManager.FindById(userId);

            if (userToDelete == null)
                throw new KeyNotFoundException($"User with id: {userId} could not be found! ");

            await _userManager.DeleteUser(userToDelete);
        }

        public async Task Update(User updatedUser, string oldEmail, string newPassword, bool isAdmin)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            Guid userId = Guid.Parse(updatedUser.Id);

            User userWithSameEmail = await _userManager.FindByEmail(updatedUser.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != updatedUser.Id)
            { // found different user with same email
                throw new EmailAddressAlreadyInUseException($"Email: {updatedUser.Email} already in use!");
            }

            if (await _userManager.FindDifferentUserWithSameUsername(userId, updatedUser.UserName) != null)
            {
                throw new UsernameTakenException($"Username: {updatedUser.UserName} already taken!");
            }

            if (isAdmin && !(await _userManager.IsUserInRole(userId, "Admin")))
            {
                await _userManager.AddRoleToUser(updatedUser, "Admin");
            }
            else if (!isAdmin && await _userManager.IsUserInRole(userId, "Admin"))
            {
                await _userManager.RemoveRoleFromUser(updatedUser, "Admin");
            }

            if (!updatedUser.Email.Equals(oldEmail))
            { // if the email gets updated, that email should be confirmed again
                updatedUser.EmailConfirmed = false;
                await EmailUserWithConfirmationToken(updatedUser);
            }

            updatedUser.ChangeDate = DateTime.Now;
            updatedUser.PasswordHash = hasher.HashPassword(updatedUser, newPassword);
            await _userManager.UpdateUser(updatedUser);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.GetAll();
        }

        public async Task<User> GetCurrentUser(ClaimsPrincipal principal)
        {
            return await _userManager.GetCurrentUser(principal);
        }

        public List<Team> GetUserTeams(User currentUser)
        {
            return currentUser.Teams.ToList();
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
            return await _userManager.IsUserInRole(Guid.Parse(currentUser.Id), "Admin");
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

        public async Task<List<User>> GetUsersUnderTeamLeader(User user)
        {
            List<User> users = new List<User>();
            List<Team> teams = _teamService.GetAllTeams().Result.Where(t => t.TeamLeader == user).ToList();

            foreach (Team t in teams)
            {
                users.AddRange(await _teamService.GetAllTeamMembers(t.Id));
            }

            return users;
        }

        public async Task ConfirmEmailAdress(string userId, string token)
        {
            User user = await GetUserById(Guid.Parse(userId));

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            await _userManager.ConfirmEmailAsync(user, normalToken);
        }
    }
}