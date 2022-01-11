using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public class AuthUserManager : UserManager<User>, IAuthUserManager
    {
        public AuthUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher,
            userValidators, passwordValidators, keyNormalizer,
            errors, services, logger)
        { }

        public async Task<List<User>> GetAllAsync()
        {
            return await Users.ToListAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            return (await GetRolesAsync(user)).ToList();
        }

        public async Task CreateUserAsync(User user, string password)
        {
            await CreateAsync(user, password);
        }

        public async Task<bool> IsUserInRole(Guid userId, string roleName)
        {
            User user = await FindByIdAsync(userId);
            return await IsInRoleAsync(user, roleName);
        }

        public async Task<User> FindByIdAsync(Guid id)
        {
            return await FindByIdAsync(id.ToString());
        }

        public async Task AddRoleToUser(User user, string role)
        {
            await AddToRoleAsync(user, role);
        }

        public async Task RemoveRoleFromUser(User user, string role)
        {
            await RemoveFromRoleAsync(user, role);
        }

        public async Task<bool> ValidateUserCredentials(string userName, string password)
        {
            User user = await FindByNameAsync(userName);
            if (user != null)
            {
                bool result = await CheckPasswordAsync(user, password);
                return result;
            }
            return false;
        }

        public async Task DeleteUserAsync(User user)
        {
            await DeleteAsync(user);
        }

        public async Task<User> FindDifferentUserWithSameUsername(Guid userId, string username)
        {
            List<User> users = await GetAllAsync();

            return users.FirstOrDefault(user =>
                user.UserName == username &&
                user.Id != userId.ToString());
        }

        public async Task EditUserAsync(User user)
        {
            await UpdateAsync(user);
        }
    }
}