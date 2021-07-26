using LibraryR.Data;
using LibraryR.Models;
using LibraryR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;



        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IUserService userService)
        {
            _logger = logger;
            _db = db;
            _userService = userService;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                DateTime d = DateTime.Now;
                ViewBag.Year = d.Year;
                ViewBag.RecordCount = getCurrentRecordCount(d.Year);
            }
            return View();
        }
        public int getCurrentRecordCount(int year)
        {
            var userId = _userService.GetUserId();
            var records = _db.Records.Where(a => a.UserId == userId && a.RecordDate.Year == year && a.StatusTypeId == 1.ToString());
            return records.Count();

        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Statistics()
        {
            var userId = _userService.GetUserId();
            var records = _db.Records.Where(a => a.UserId == userId);
            DateTime dateTime = DateTime.Now;
            var currentYearCount = records.Where(a => a.RecordDate.Year == dateTime.Year && a.StatusTypeId == 1.ToString()).Count();
            var last1YearCount = records.Where(a => a.RecordDate.Year == dateTime.Year - 1 && a.StatusTypeId == 1.ToString()).Count();
            var last2YearCount = records.Where(a => a.RecordDate.Year == dateTime.Year - 2 && a.StatusTypeId == 1.ToString()).Count();
            var last3YearCount = records.Where(a => a.RecordDate.Year == dateTime.Year - 3 && a.StatusTypeId == 1.ToString()).Count();
            var last4YearCount = records.Where(a => a.RecordDate.Year == dateTime.Year - 4 && a.StatusTypeId == 1.ToString()).Count();
            var last5YearCount = records.Where(a => a.RecordDate.Year == dateTime.Year - 5 && a.StatusTypeId == 1.ToString()).Count();

            var currentYearMonthly = currentYearCount / (Convert.ToDouble(dateTime.Month));
            var year1Monthly = last1YearCount / 12.0;
            var year2Monthly = last2YearCount / 12.0;
            var year3Monthly = last3YearCount / 12.0;
            var year4Monthly = last4YearCount / 12.0;
            var year5Monthly = last5YearCount / 12.0;

            List<KeyValuePair<int, double>> currentInfo = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> lastInfo = new List<KeyValuePair<int, double>>();
            lastInfo.Add(new KeyValuePair<int, double>(currentYearCount, Math.Round(currentYearMonthly, 2, MidpointRounding.AwayFromZero)));
            lastInfo.Add(new KeyValuePair<int, double>(last1YearCount,  Math.Round(year1Monthly,2,MidpointRounding.AwayFromZero)));
            lastInfo.Add(new KeyValuePair<int, double>(last2YearCount,  Math.Round(year2Monthly,2,MidpointRounding.AwayFromZero)));
            lastInfo.Add(new KeyValuePair<int, double>(last3YearCount,  Math.Round(year3Monthly,2,MidpointRounding.AwayFromZero)));
            lastInfo.Add(new KeyValuePair<int, double>(last4YearCount,  Math.Round(year4Monthly,2,MidpointRounding.AwayFromZero)));
            lastInfo.Add(new KeyValuePair<int, double>(last5YearCount, Math.Round(year5Monthly, 2, MidpointRounding.AwayFromZero)));

            ViewBag.LastInfo = lastInfo;
            ViewBag.Year = dateTime.Year;
            List<int> years = new List<int> { 2016, 2017, 2018, 2019, 2020, 2021 };
            List<int> countss = new List<int> { last5YearCount, last4YearCount, last3YearCount, last2YearCount, last1YearCount, currentYearCount };
            ViewBag.Ye = JsonSerializer.Serialize(years);
            ViewBag.Ct = JsonSerializer.Serialize(countss);
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStatus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStatus(StatusType statusType)
        {
            _db.StatusTypes.Add(statusType);
            _db.SaveChanges();

            return RedirectToAction("ListStatus");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteStatus(int? id)
        {
            var status = _db.StatusTypes.Find(id);
            if (status == null)
                return NotFound();
            return View(status);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteStatusConfirmed(int id)
        {
            var status = _db.StatusTypes.Find(id);
            if (status == null)
                return NotFound();
            _db.StatusTypes.Remove(status);
            _db.SaveChanges();

            return RedirectToAction("ListStatus");
        }
        public IActionResult ListStatus()
        {
            var status = _db.StatusTypes;
            return View(status);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditStatus(int? id)
        {
            var status = _db.StatusTypes.Find(id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult EditStatus(StatusType statusType)
        {
            _db.StatusTypes.Update(statusType);
            _db.SaveChanges();
            return RedirectToAction("ListStatus");
        }
        public IActionResult DetailsStatus(int? id)
        {
            var status = _db.StatusTypes.Find(id);
            if (status != null)
                return View(status);
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
