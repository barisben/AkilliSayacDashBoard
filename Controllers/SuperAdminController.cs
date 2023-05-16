using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;
            log.LogMessage = "Kullanıcının hesabı yönetici tarafından silindi.";

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
            var databaseLogs = _db.Logs.Where(x => x.LogTypeId == databaseLogsTypeId).ToList();
            return View(databaseLogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DatabaseBackUp(string id)
        {
            try
            {
                string databaseName = _db.Database.GetDbConnection().Database;
                string backupFileName = Path.Combine(Path.GetTempPath(), $"{databaseName}_backup_{DateTime.Now:yyyyMMddHHmmss}.bak");

                _db.Database.ExecuteSqlRaw($"BACKUP DATABASE {_db.Database.GetDbConnection().Database} TO DISK = '{backupFileName}'");

                byte[] fileBytes = System.IO.File.ReadAllBytes(backupFileName);
                System.IO.File.Delete(backupFileName);

                string downloadFileName = $"{databaseName}_backup_{DateTime.Now:yyyyMMddHHmmss}.bak";

                Log log = new Log();
                log.LogMessage = "Veritabanı yedeği indirildi";
                log.LogTime = DateTime.Now;
                log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "Database").FirstOrDefault().LogTypeId;
                log.UserId = id;

                _db.Logs.Add(log);
                _db.SaveChanges();

                return File(fileBytes, "application/octet-stream", downloadFileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}