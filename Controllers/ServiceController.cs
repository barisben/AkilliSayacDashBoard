using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkilliSayac.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        public IActionResult Anomaly()
        {
            return View();
        }
        public IActionResult Log()
        {
            return View();
        }
        public IActionResult Malware()
        {
            return View();
        }
    }
}
