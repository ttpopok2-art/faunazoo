using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;
using WebApplication8.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace WebApplication8.Controllers
{
    public class EnclosuresController : Controller
    {
        private readonly FaunaContext _context;
        private readonly IWebHostEnvironment _env;

        public EnclosuresController(FaunaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ThenInclude(a => a.Species)
                .ToList();
            return View(enclosures);
        }

        public IActionResult Create()
        {
            ViewBag.Animals = _context.Animals
                .Include(a => a.Species)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Enclosures enclosure, int[]? SelectedAnimalIds, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Animals = _context.Animals.Include(a => a.Species).ToList();
                return View(enclosure);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "enclosures");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                enclosure.ImageUrl = $"/images/enclosures/{fileName}";
            }

            _context.Enclosures.Add(enclosure);
            _context.SaveChanges();

            if (SelectedAnimalIds != null)
            {
                foreach (var animalId in SelectedAnimalIds)
                {
                    var animal = _context.Animals.FirstOrDefault(a => a.Animal_ID == animalId);
                    if (animal != null)
                    {
                        animal.Enclosure_ID = enclosure.Enclosure_ID;
                        _context.Animals.Update(animal);
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var enclosure = _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefault(e => e.Enclosure_ID == id);

            if (enclosure == null) return NotFound();

            ViewBag.Animals = _context.Animals.Include(a => a.Species).ToList();
            ViewBag.SelectedAnimalIds = enclosure.Animals.Select(a => a.Animal_ID).ToList(); // ✅ Список выбранных животных

            return View(enclosure);
        }

        [HttpPost]
        public IActionResult Edit(Enclosures enclosure, int[]? SelectedAnimalIds, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Animals = _context.Animals.Include(a => a.Species).ToList();
                ViewBag.SelectedAnimalIds = SelectedAnimalIds ?? new int[0];
                return View(enclosure);
            }

            var existing = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Enclosure_ID == enclosure.Enclosure_ID);

            if (existing == null) return NotFound();

            existing.Type = enclosure.Type;
            existing.Location = enclosure.Location;
            existing.Size = enclosure.Size;
            existing.Capacity = enclosure.Capacity;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "enclosures");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                existing.ImageUrl = $"/images/enclosures/{fileName}";
            }

            var currentAnimalIds = existing.Animals.Select(a => a.Animal_ID).ToList();
            var selectedIds = SelectedAnimalIds?.ToList() ?? new List<int>();

            var animalsToRemove = currentAnimalIds.Except(selectedIds).ToList();
            foreach (var animalId in animalsToRemove)
            {
                var animal = _context.Animals.FirstOrDefault(a => a.Animal_ID == animalId);
                if (animal != null)
                    animal.Enclosure_ID = null;
            }

            var animalsToAdd = selectedIds.Except(currentAnimalIds).ToList();
            foreach (var animalId in animalsToAdd)
            {
                var animal = _context.Animals.FirstOrDefault(a => a.Animal_ID == animalId);
                if (animal != null)
                    animal.Enclosure_ID = existing.Enclosure_ID;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Enclosure_ID == id);
            if (enclosure == null) return NotFound();
            return View(enclosure);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var enclosure = _context.Enclosures.Find(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
