using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkilliSayac.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Create()
        {
            return View(new IdentityRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            await roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }

        public IActionResult UserList()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }
    }
}