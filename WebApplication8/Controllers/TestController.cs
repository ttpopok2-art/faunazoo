using Microsoft.AspNetCore.Mvc;
using WebApplication8.Data;

namespace WebApplication8.Controllers
{
    public class TestController : Controller
    {
        private readonly FaunaContext _context;

        public TestController(FaunaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var speciesCount = _context.Species.Count();
                return Content($"✅ Подключение работает! В таблице Species записей: {speciesCount}");
            }
            catch (Exception ex)
            {
                return Content($"❌ Ошибка подключения: {ex.Message}");
            }
        }
    }
}
