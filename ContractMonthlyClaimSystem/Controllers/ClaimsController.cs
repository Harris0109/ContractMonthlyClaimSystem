using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    [AllowAnonymous] // This line allows access without login
    public class ClaimsController : Controller
    {

        // GET: /Claims/ (Will show a list of claims)
        public IActionResult Index()
        {
            return View(); // This will look for a view called "Index.cshtml" in /Views/Claims/
        }

        // GET: /Claims/Create (Will show the form to create a new claim)
        public IActionResult Create()
        {
            return View(); // This will look for a view called "Create.cshtml"
        }

        // GET: /Claims/Details/5 (Will show details for a specific claim with ID=5)
        public IActionResult Details(int id)
        {
            return View(); // This will look for a view called "Details.cshtml"
        }
    }
}
