using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ContractMonthlyClaimSystem.Services;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ClaimValidationService _validationService;

        public ClaimsController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _validationService = new ClaimValidationService();
        }

        // GET: /Claims/
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"=== INDEX: User ID: {userId} ===");

                var userClaims = await _context.Claims
                    .Where(c => c.LecturerId == userId)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                Console.WriteLine($"=== INDEX: Found {userClaims.Count} claims ===");

                // REMOVE the test claim creation - we want real data only
                return View(userClaims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== INDEX ERROR: {ex.Message} ===");
                return View(new List<MonthlyClaims>());
            }
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonthlyClaims claim)
        {
            Console.WriteLine("=== CREATE ACTION STARTED ===");

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("=== MODEL IS VALID ===");

                    // Auto-calculate total amount
                    claim.CalculateTotal();
                    Console.WriteLine($"Calculated Total: {claim.TotalAmount}");

                    // Get current user
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Console.WriteLine($"User ID: {userId}");

                    claim.LecturerId = userId;
                    claim.Status = "Pending";
                    claim.SubmittedDate = DateTime.Now;

                    Console.WriteLine($"Claim before save - Hours: {claim.TotalHours}, Rate: {claim.HourlyRate}, Total: {claim.TotalAmount}");

                    // Handle file upload
                    if (claim.UploadedFile != null && claim.UploadedFile.Length > 0)
                    {
                        Console.WriteLine("File upload detected");
                        var documentsFolder = Path.Combine(_hostEnvironment.WebRootPath, "documents");

                        if (!Directory.Exists(documentsFolder))
                        {
                            Directory.CreateDirectory(documentsFolder);
                        }

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

                        claim.Documents = new List<SupportingDocument> { document };
                        Console.WriteLine("File saved successfully");
                    }

                    // SAVE THE CLAIM
                    _context.Claims.Add(claim);
                    int result = await _context.SaveChangesAsync();

                    Console.WriteLine($"=== SAVE SUCCESSFUL! Records affected: {result}, New Claim ID: {claim.ClaimId} ===");

                    TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} submitted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== SAVE ERROR: {ex.Message} ===");
                    Console.WriteLine($"=== STACK TRACE: {ex.StackTrace} ===");
                    ModelState.AddModelError("", $"Error saving claim: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("=== MODEL INVALID ===");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model Error: {error.ErrorMessage}");
                }
            }

            return View(claim);
        }

        // GET:/Claims/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        //GET: /Claims/Review (For Coordinators/Managers to review claims)
        // ENHANCE the Review method
        [Authorize(Roles = "Coordinator,Manager")]
        public async Task<IActionResult> Review()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();

            // Run validation on all pending claims
            foreach (var claim in pendingClaims)
            {
                claim.RunValidation();
            }

            return View(pendingClaims);
        }

        //POST: /Claims/Approve/5
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

        // POST: /Claims/Reject/5
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

        // ADD automated bulk approval
        [HttpPost]
        [Authorize(Roles = "Coordinator,Manager")]
        public async Task<IActionResult> BulkApproveValid()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();

            int approvedCount = 0;

            foreach (var claim in pendingClaims)
            {
                var (isValid, message) = _validationService.ValidateClaim(claim);

                if (isValid)
                {
                    claim.Status = "Approved";
                    approvedCount++;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Automatically approved {approvedCount} claims that passed validation!";
            return RedirectToAction(nameof(Review));
        }
    }
}