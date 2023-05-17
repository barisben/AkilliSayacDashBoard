using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using AkilliSayac.Database;
using AkilliSayac.Interfaces;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace AkilliSayac.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private UserManager<AkilliSayacUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IDatabaseOperation database;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<AkilliSayacUser> userManager, ApplicationDbContext db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._db = db;

            //for Sql Server
            this.database = new DatabaseOperation(new SqlServerDb(_db));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;
            log.LogMessage = "Kullanıcının hesabı yönetici tarafından silindi.";
            log.LogStatusBadge = "badge bg-warning text-dark";
            log.LogStatus = "Yönetim";

            _db.Logs.Add(log);
            _db.SaveChanges();

            return RedirectToAction("UserList", "SuperAdmin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLock(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;
            log.LogMessage = "Kullanıcının blokesi yönetici tarafından kaldırıldı.";
            log.LogStatusBadge = "badge bg-warning text-dark";
            log.LogStatus = "Yönetim";

            _db.Logs.Add(log);

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

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;
            log.LogMessage = "Kullanıcı yönetici tarafından bloklandı.";
            log.LogStatusBadge = "badge bg-warning text-dark";
            log.LogStatus = "Yönetim";

            _db.Logs.Add(log);

            await _db.SaveChangesAsync();
            return RedirectToAction("UserList", "SuperAdmin");
        }

        public IActionResult UserList()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }

        public IActionResult DatabaseActions()
        {
            int databaseLogsTypeId = _db.LogTypes.Where(x => x.LogTypeName == "Database").FirstOrDefault().LogTypeId;
            ViewData["DatabaseLogs"] = _db.Logs.Where(x => x.LogTypeId == databaseLogsTypeId).ToList();
            ViewData["Users"] = userManager.Users.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DatabaseBackUp(string id)
        {
            try
            {
                string databaseName = _db.Database.GetDbConnection().Database;

                //for Sql Server
                byte[] backUpFileBytes = database.DatabaseBackUp(id);
                string downloadFileName = $"{databaseName}_backup_{DateTime.Now:ddMMyyyyHHmmss}.bak";

                return File(backUpFileBytes, "application/octet-stream", downloadFileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}