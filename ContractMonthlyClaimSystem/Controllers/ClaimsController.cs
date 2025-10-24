using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClaimsController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Claims/
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // FIX: Change LecturerId to string and remove .ToString()
            var userClaims = await _context.Claims
                .Where(c => c.LecturerId == userId)
                .ToListAsync();
            return View(userClaims);
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonthlyClaims claim)
        {
            if (ModelState.IsValid)
            {
                claim.Status = "Pending";
                claim.SubmittedDate = DateTime.Now;

                // FIX: Use actual user ID instead of hardcoded 1
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                claim.LecturerId = userId; // Now string to string

                // File upload logic remains the same...
                if (claim.UploadedFile != null && claim.UploadedFile.Length > 0)
                {
                    var documentsFolder = Path.Combine(_hostEnvironment.WebRootPath, "documents");
                    if (!Directory.Exists(documentsFolder))
                        Directory.CreateDirectory(documentsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + claim.UploadedFile.FileName;
                    var filePath = Path.Combine(documentsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await claim.UploadedFile.CopyToAsync(fileStream);
                    }

                    var document = new SupportingDocument
                    {
                        OriginalFileName = claim.UploadedFile.FileName,
                        FilePath = uniqueFileName,
                        UploadedDate = DateTime.Now,
                        ClaimId = claim.ClaimId
                    };

                    claim.Documents ??= new List<SupportingDocument>();
                    claim.Documents.Add(document);
                }

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // Other methods remain the same...
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        [Authorize(Roles = "Coordinator,Manager")]
        public async Task<IActionResult> Review()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();
            return View(pendingClaims);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = "Approved";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Review));
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator,Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = "Rejected";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Review));
        }
    }
}