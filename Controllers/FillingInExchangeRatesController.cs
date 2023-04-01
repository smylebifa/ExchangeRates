using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class FillingInExchangeRatesController : Controller
    {
        public IActionResult Index(string status = "")
        {
            ViewBag.Status = status;
            return View();
        }

        [HttpPost("/save_exchange_rates")]
        public IActionResult SaveExchangeRatesByPeriodInDB()
        {
            return RedirectToAction("Index", "FillingInExchangeRates", new { status = "success" });
        }
    }
}
