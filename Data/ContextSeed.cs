using AkilliSayac.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace AkilliSayac.Data
{
    public static class ContextSeed
    {
        public static void Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
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

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if(userManager.FindByEmailAsync("baris@sau.edu.tr").Result == null)
            {
                var user = new IdentityUser
                {
                    UserName = "baris@sau.edu.tr",
                    Email = "baris@sau.edu.tr",
                    EmailConfirmed = true,
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
