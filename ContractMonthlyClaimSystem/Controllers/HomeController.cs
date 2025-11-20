using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; // ADD THIS
using ContractMonthlyClaimSystem.Data; // ADD THIS

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // ADD THIS

        // ADD THIS CONSTRUCTOR
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ADD THIS ACTION METHOD
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var claimsCount = await _context.Claims.CountAsync();
                var lecturersCount = await _context.Lecturers.CountAsync();

                ViewBag.ClaimsCount = claimsCount;
                ViewBag.LecturersCount = lecturersCount;
                ViewBag.Message = "Database connection successful!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Database error: {ex.Message}";
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}