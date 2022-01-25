using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ITeamService _teamService;
        private readonly IAuthUserManager _userManager;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;


        public UserService(ITeamService teamService, IAuthUserManager userManager, IMailService mailService, IConfiguration configuration)
        {
            _teamService = teamService;
            _userManager = userManager;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task Add(User user, string password, bool isAdmin)
        {
            if (await _userManager.FindDifferentUserWithSameUsername(Guid.Parse(user.Id), user.UserName) != null)
            {
                throw new UsernameTakenException($"Username: {user.UserName} already taken!");
            }

            await IsEmailValid(Guid.Parse(user.Id), user.Email);

            user.TwoFactorEnabled = true;
            await _userManager.CreateUser(user, password);

            await EmailUserWithConfirmationToken(user);

            if (isAdmin)
            {
                await _userManager.AddRoleToUser(user, "Admin");
            }
        }

        private async Task EmailUserWithConfirmationToken(User user)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string appUrl = $"{_configuration["AppUrl"]}";
            string url = $"{appUrl}/api/User/confirmemail?userid={user.Id}&token={validEmailToken}";

            await _mailService.SendEmail(new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Confirm your email",
                Body = $"<p>Confirm your email by <a href='{url}'> Clicking Here </a>  </p>"
            });
        }

        private async Task IsEmailValid(Guid id, string email)
        {
            try
            {
                var mail = new MailAddress(email);

                var userAlreadyInDb = await _userManager.FindById(id);
                var userWithSameEmail = await _userManager.FindByEmail(email);

                if ((userWithSameEmail != null && userAlreadyInDb == null) || (userWithSameEmail != null && userWithSameEmail.Id != userAlreadyInDb.Id)) //Checks if email is valid when just updating the user or creating a new one
                {
                    throw new EmailAddressAlreadyInUseException($"Email: {email} already in use!");
                }
            }
            catch (FormatException)
            {
                throw new InvalidEmailException($"Email: {email} is not valid!");
            }
        }

        public async Task Delete(Guid id)
        {
            var user = await _userManager.FindById(id);

            if (user == null)
                throw new KeyNotFoundException($"User with id: {id} could not be found! ");

            await _userManager.DeleteUser(user);
        }

        public async Task Update(User user, string oldEmail, string newPassword, bool isAdmin)
        {
            var hasher = new PasswordHasher<User>();
            var userId = Guid.Parse(user.Id);

            await IsEmailValid(userId, user.Email);

            if (await _userManager.FindDifferentUserWithSameUsername(userId, user.UserName) != null)
            {
                throw new UsernameTakenException($"Username: {user.UserName} already taken!");
            }

            if (isAdmin && !(await _userManager.IsUserInRole(userId, "Admin")))
            {
                await _userManager.AddRoleToUser(user, "Admin");
            }
            else if (!isAdmin && await _userManager.IsUserInRole(userId, "Admin"))
            {
                await _userManager.RemoveRoleFromUser(user, "Admin");
            }

            if (!user.Email.Equals(oldEmail))
            { // if the email gets updated, that email should be confirmed again
                user.EmailConfirmed = false;
                await EmailUserWithConfirmationToken(user);
            }

            user.ChangeDate = DateTime.Now;
            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _userManager.UpdateUser(user);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.GetAll();
        }

        public async Task<User> GetCurrentUser(ClaimsPrincipal principal)
        {
            return await _userManager.GetCurrentUser(principal);
        }

        public List<Team> GetUserTeams(User user)
        {
            return user.Teams.ToList();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userManager.FindById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with id: {id} does not exist!");
            return user;
        }

        public async Task<bool> IsUserAdmin(User user)
        {
            return await _userManager.IsUserInRole(Guid.Parse(user.Id), "Admin");
        }

        public async Task MakeUserAdmin(Guid id)
        {
            var user = await GetUserById(id);

            if (!(await _userManager.IsUserInRole(Guid.Parse(user.Id), "Admin")))
            {
                await _userManager.AddRoleToUser(user, "Admin");
            }
        }

        public async Task RemoveUserFromAdmin(Guid id)
        {
            var user = await GetUserById(id);

            if (await _userManager.IsUserInRole(Guid.Parse(user.Id), "Admin"))
            {
                await _userManager.RemoveRoleFromUser(user, "Admin");
            }
        }

        public async Task<List<User>> GetUsersUnderTeamLeader(User user)
        {
            var users = new List<User>();
            var teams = _teamService.GetAllTeams().Result.Where(t => t.TeamLeader == user).ToList();

            foreach (Team t in teams)
            {
                users.AddRange(await _teamService.GetAllTeamMembers(t.Id));
            }

            return users;
        }

        public async Task ConfirmEmailAdress(string userId, string token)
        {
            var user = await GetUserById(Guid.Parse(userId));

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            await _userManager.ConfirmEmailAsync(user, normalToken);
        }
    }
}