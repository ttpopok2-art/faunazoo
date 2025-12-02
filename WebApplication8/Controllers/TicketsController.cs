using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class TicketsController : Controller
    {
        private readonly FaunaContext _context;

        public TicketsController(FaunaContext context)
        {
            _context = context;
        }

        // ======================= INDEX =======================
        public IActionResult Index(string role = "user")
        {
            // Если админ — показываем все билеты
            if (role == "admin")
            {
                var allTickets = _context.Tickets
                    .Include(t => t.Visitor)
                    .OrderByDescending(t => t.Sale_date)
                    .ToList();
                ViewBag.Role = "admin";
                return View(allTickets);
            }

            // Если пользователь — показываем только его билеты
            // (Для примера просто фильтруем по первому посетителю)
            var visitor = _context.Visitors.FirstOrDefault();
            if (visitor == null)
            {
                ViewBag.Message = "Нет данных о пользователях.";
                return View(new List<Tickets>());
            }

            var userTickets = _context.Tickets
                .Include(t => t.Visitor)
                .Where(t => t.Visitor_ID == visitor.Visitor_ID)
                .OrderByDescending(t => t.Sale_date)
                .ToList();

            ViewBag.Role = "user";
            return View(userTickets);
        }

        // ======================= CREATE (GET) =======================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Visitors = _context.Visitors.ToList();
            return View();
        }

        // ======================= CREATE (POST) =======================
        [HttpPost]
        public IActionResult Create(Tickets ticket)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Visitors = _context.Visitors.ToList();
                return View(ticket);
            }

            ticket.Sale_date = DateTime.Now; // Дата продажи автоматически
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { role = "admin" });
        }

        // ======================= DELETE (GET) =======================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.Visitor)
                .FirstOrDefault(t => t.Ticket_ID == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // ======================= DELETE (POST) =======================
        [HttpPost]
        public IActionResult DeleteConfirmed(int Ticket_ID)
        {
            var ticket = _context.Tickets.Find(Ticket_ID);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index), new { role = "admin" });
        }
    }
}
