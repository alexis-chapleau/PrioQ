using Microsoft.AspNetCore.Mvc;

namespace PrioQ.Presentation.Controllers // or your namespace
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Possibly return a landing/Accueil page
            return View();
        }
    }
}
