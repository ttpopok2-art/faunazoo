using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly FaunaContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(FaunaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var employees = _context.Employees
                .Include(e => e.Enclosure)
                .OrderBy(e => e.Employee_ID)
                .ToList();

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Enclosures = _context.Enclosures.ToList();
            var employee = new Employees(); // пустая модель для Razor
            return View(employee);
        }

        [HttpPost]
        public IActionResult Create(Employees employee, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Enclosures = _context.Enclosures.ToList();
                var errors = string.Join("<br>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.ModelErrors = errors;
                return View(employee);
            }

            // Сохраняем изображение, если есть
            if (imageFile != null && imageFile.Length > 0)
            {
                string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadsDir = Path.Combine(webRootPath, "images/employees");
                Directory.CreateDirectory(uploadsDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                employee.ImageUrl = "/images/employees/" + fileName;
            }

            _context.Employees.Add(employee);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();

            ViewBag.Enclosures = _context.Enclosures.ToList();
            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(Employees employee, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsDir = Path.Combine(_env.WebRootPath, "images/employees");
                    Directory.CreateDirectory(uploadsDir);

                    string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    employee.ImageUrl = "/images/employees/" + fileName;
                }

                _context.Employees.Update(employee);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Enclosures = _context.Enclosures.ToList();
            return View(employee);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees
                .Include(e => e.Enclosure)
                .FirstOrDefault(e => e.Employee_ID == id);

            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp != null)
            {
                _context.Employees.Remove(emp);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
