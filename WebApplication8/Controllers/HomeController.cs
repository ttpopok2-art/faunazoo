using Microsoft.AspNetCore.Mvc;

namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
