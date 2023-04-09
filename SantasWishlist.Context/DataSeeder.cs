using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWishlist.Context
{
    public static class DataSeeder
    {
        
        public static void SeedRolesAndUsers(RoleManager<IdentityRole> roleManager, UserManager<SantasWishlistUser> userManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);            
        }
        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            List<string> roles = new()
            {
                "Santa",
                "Child"
            };

            foreach(var roleName in roles)
            {
                CreateRole(roleManager, roleName);
            }
        }

        private static void CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
            {
                IdentityRole role = new();
                role.Name = roleName;
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
        private static void SeedUsers(UserManager<SantasWishlistUser> userManager)
        {
            CreateUser(userManager, "Kerstman", "K3rst!", roleName: "Santa");
            CreateUser(userManager, "Pietje", "bel", roleName: "Child");
        }

        private static void CreateUser(UserManager<SantasWishlistUser> userManager, string userName, string password, string roleName = "")
        {
            var hasher = new PasswordHasher<IdentityUser>();

            if (userManager.FindByNameAsync(userName).Result == null)
            {
                SantasWishlistUser user = new();
                user.Email = userName + "@santa.np";
                user.UserName = userName;
                user.PasswordHash = hasher.HashPassword(user, password);
                user.WasGood = true;
                user.SentWishlist = false;

                IdentityResult userResult = userManager.CreateAsync(user).Result;

                if (userResult.Succeeded && !String.IsNullOrEmpty(roleName))
                {
                    AddUserToRole(userManager, user, roleName);
                }                
            }
            
        }

        private static void AddUserToRole(UserManager<SantasWishlistUser> userManager, SantasWishlistUser user, string roleName) 
        {
            userManager.AddToRoleAsync(user, roleName).Wait();
        }

        
    }
}
