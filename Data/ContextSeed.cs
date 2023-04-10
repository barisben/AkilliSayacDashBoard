using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;

namespace AkilliSayac.Data
{
    [Authorize]
    public static class ContextSeed
    {
        public static void Seed(UserManager<AkilliSayacUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "SuperAdmin"
                };
                var result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                var result = roleManager.CreateAsync(role);
            }
        }

        private static void SeedUsers(UserManager<AkilliSayacUser> userManager)
        {
            if(userManager.FindByEmailAsync("baris@sau.edu.tr").Result == null)
            {
                var user = new AkilliSayacUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "baris@sau.edu.tr",
                    Email = "baris@sau.edu.tr",
                    RoleName = "SuperAdmin",
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(user, "P@ssword1").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "SuperAdmin").Wait();
                }
            }
        }
    }
}
