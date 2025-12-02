using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class SpeciesController : Controller
    {
        private readonly FaunaContext _context;
        private readonly IWebHostEnvironment _environment;

        public SpeciesController(FaunaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index(string search, string[]? filterTypes)
        {
            var query = _context.Species.AsQueryable();

            // 🔍 Поиск по названию и описанию
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.Species_name.ToLower().Contains(search) ||
                    (s.Description != null && s.Description.ToLower().Contains(search)));
            }

            // 🎯 Фильтрация по видам (множественный выбор)
            if (filterTypes != null && filterTypes.Length > 0)
            {
                query = query.Where(s =>
                    (filterTypes.Contains("rare") && s.IsRare) ||
                    (filterTypes.Contains("endangered") && s.IsEndangered) ||
                    (filterTypes.Contains("normal") && !s.IsRare && !s.IsEndangered)
                );
            }

            ViewBag.SelectedTypes = filterTypes;
            return View(query.ToList());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Species species, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    species.ImageUrl = "/images/" + uniqueFileName;
                }

                _context.Species.Add(species);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(species);
        }

        public IActionResult Edit(int id)
        {
            var species = _context.Species.Find(id);
            if (species == null) return NotFound();
            return View(species);
        }

        [HttpPost]
        public IActionResult Edit(Species species, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Species.AsNoTracking().FirstOrDefault(s => s.Species_ID == species.Species_ID);
                if (existing == null) return NotFound();

                if (ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    species.ImageUrl = "/images/" + uniqueFileName;
                }
                else
                {
                    species.ImageUrl = existing.ImageUrl;
                }

                _context.Update(species);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(species);
        }

        public IActionResult Delete(int id)
        {
            var species = _context.Species.Find(id);
            if (species == null) return NotFound();
            return View(species);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var species = _context.Species.Find(id);
            if (species != null)
            {
                _context.Species.Remove(species);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
