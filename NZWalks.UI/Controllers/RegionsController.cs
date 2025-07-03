using Microsoft.AspNetCore.Mvc;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        public IActionResult Index()
        {
            // Get all regions from web api
            return View();
        }
    }
}
