using AkilliSayac.Areas.Identity.Data;
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
        private UserManager<AkilliSayacUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<AkilliSayacUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);
            return RedirectToAction("UserList", "SuperAdmin");
        }

        public IActionResult UserList()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }
    }
}