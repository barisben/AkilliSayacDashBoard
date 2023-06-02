using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkilliSayac.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private UserManager<AkilliSayacUser> userManager;

        public ServiceController(ApplicationDbContext db, UserManager<AkilliSayacUser> userManager)
        {
            _db = db;
            this.userManager = userManager;
        }

        public async Task<IActionResult> ThreatIntelligenceAsync()
        {
            return View();
        }

        public async Task<IActionResult> AnomalyAsync()
        {
            ViewData["Anomalies"] = await _db.Anomalies.ToListAsync();
            ViewData["AnomalyTypes"] = await _db.AnomalyTypes.ToListAsync();
            return View();
        }
        public async Task<IActionResult> LogAsync()
        {
            ViewData["Logs"] = await _db.Logs.ToListAsync();
            ViewData["LogTypes"] = await _db.LogTypes.ToListAsync();
            ViewData["Devices"] = await _db.Devices.ToListAsync();
            ViewData["Users"] = await userManager.Users.ToListAsync();
            return View();
        }
        public async Task<IActionResult> MalwareAsync()
        {
            ViewData["Malwares"] = await _db.Malwares.ToListAsync();
            ViewData["MalwareTypes"] = await _db.MalwareTypes.ToListAsync();
            ViewData["Devices"] = await _db.Devices.ToListAsync();
            return View();
        }
    }
}
