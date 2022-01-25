using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.WEB
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAuthUserManager _userManager;

        public PasswordValidator(IAuthUserManager userManager)
        {
            _userManager = userManager;
        }

        //This method validates the user credentials and if successful teh IdentiryServer will build a token from the context.Result object
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            User user;

            if (context.UserName.Contains("@"))
            {
                user = await _userManager.FindByEmail(context.UserName);
            }
            else
            {
                user = await _userManager.FindByName(context.UserName);
            }

            if (user != null)
            {
                bool authResult = await _userManager.ValidateUserCredentials(context.UserName, context.Password);
                if (authResult)
                {
                    List<string> roles = await _userManager.GetUserRoles(user);

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    context.Result = new GrantValidationResult(subject: user.Id, authenticationMethod: "password", claims: claims);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
                }

                return;
            }
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
        }
    }
}