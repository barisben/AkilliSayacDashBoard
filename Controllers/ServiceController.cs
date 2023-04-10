﻿using AkilliSayac.Data;
using Microsoft.AspNetCore.Authorization;
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

        public ServiceController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Anomaly()
        {
            return View();
        }
        public async Task<IActionResult> LogAsync()
        {
            ViewData["Logs"] = await _db.Logs.ToListAsync();
            ViewData["LogTypes"] = await _db.LogTypes.ToListAsync();
            ViewData["Devices"] = await _db.Devices.ToListAsync();
            return View();
        }
        public IActionResult Malware()
        {
            return View();
        }
    }
}
