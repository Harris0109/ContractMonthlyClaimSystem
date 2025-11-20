using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize] // Only managers can access HR functions
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HR Dashboard
        public async Task<IActionResult> Index()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == "Approved")
                .ToListAsync(); // REMOVED: .Include(c => c.Lecturer)

            var totalAmount = approvedClaims.Sum(c => c.TotalAmount);
            var claimCount = approvedClaims.Count;

            ViewBag.TotalAmount = totalAmount;
            ViewBag.ClaimCount = claimCount;

            return View(approvedClaims);
        }

        // GET: Generate Payment Report
        public async Task<IActionResult> GeneratePaymentReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == "Approved")
                .ToListAsync(); // REMOVED: .Include(c => c.Lecturer) and .OrderBy(c => c.Lecturer.LastName)

            ViewBag.ReportDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewBag.TotalAmount = approvedClaims.Sum(c => c.TotalAmount);

            return View(approvedClaims);
        }

        // GET: Manage Lecturers
        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _context.Lecturers
                .OrderBy(l => l.LastName)
                .ToListAsync();

            return View(lecturers);
        }

        // POST: Update Lecturer
        [HttpPost]
        public async Task<IActionResult> UpdateLecturer(int id, string firstName, string lastName, string email, decimal hourlyRate)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer != null)
            {
                lecturer.FirstName = firstName;
                lecturer.LastName = lastName;
                lecturer.Email = email;
                lecturer.HourlyRate = hourlyRate;

                _context.Lecturers.Update(lecturer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Lecturer updated successfully!";
            }

            return RedirectToAction(nameof(ManageLecturers));
        }

        //Test
        public async Task<IActionResult> CreateTestHRData()
        {
            try
            {
                // Create test approved claims
                var approvedClaims = new List<MonthlyClaims>
        {
            new MonthlyClaims
            {
                Month = 1,
                Year = 2025,
                TotalHours = 40,
                HourlyRate = 150,
                TotalAmount = 6000,
                Status = "Pending",
                SubmittedDate = DateTime.Now.AddDays(-5),
                LecturerId = "test-lecturer-1"
            },
            new MonthlyClaims
            {
                Month = 1,
                Year = 2025,
                TotalHours = 35,
                HourlyRate = 200,
                TotalAmount = 7000,
                Status = "Approved",
                SubmittedDate = DateTime.Now.AddDays(-3),
                LecturerId = "test-lecturer-2"
            },
            new MonthlyClaims
            {
                Month = 2,
                Year = 2025,
                TotalHours = 45,
                HourlyRate = 180,
                TotalAmount = 8100,
                Status = "Approved",
                SubmittedDate = DateTime.Now.AddDays(-1),
                LecturerId = "test-lecturer-3"
            }
        };

                _context.Claims.AddRange(approvedClaims);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Test HR data created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating test data: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}