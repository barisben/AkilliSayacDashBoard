using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
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
        private readonly ApplicationDbContext _db;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<AkilliSayacUser> userManager, ApplicationDbContext db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);
            return RedirectToAction("UserList", "SuperAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLock(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            await _db.SaveChangesAsync();
            return RedirectToAction("UserList", "SuperAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            user.LockoutEnd = DateTime.Now.AddYears(100);
            user.AccessFailedCount = 5;
            await _db.SaveChangesAsync();
            return RedirectToAction("UserList", "SuperAdmin");
        }

        public IActionResult UserList()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }
    }
}