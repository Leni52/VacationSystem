using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DAL.Data
{
    public class DatabaseSeeder
    {
        public static void Seed(IServiceProvider applicationServices)
        {
            using (IServiceScope serviceScope = applicationServices.CreateScope())
            {
                DatabaseContext context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                if (context.Database.EnsureCreated())
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();

                    User admin = new User()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Email = "admin@test.test",
                        NormalizedEmail = "admin@test.test".ToUpper(),
                        EmailConfirmed = true,
                        UserName = "admin",
                        NormalizedUserName = "admin".ToUpper(),
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };

                    admin.PasswordHash = hasher.HashPassword(admin, "adminpass");

                    IdentityRole identityRoleAdmin = new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")

                    };

                    IdentityUserRole<string> identityUserRoleAdmin = new IdentityUserRole<string>() { 
                        RoleId = identityRoleAdmin.Id, UserId = admin.Id 
                    };

                    context.Roles.Add(identityRoleAdmin);
                    context.Users.Add(admin);
                    context.UserRoles.Add(identityUserRoleAdmin);
                    context.SaveChanges();
                }
            }
        }
    }
}
