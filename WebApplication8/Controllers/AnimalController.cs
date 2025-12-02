using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly FaunaContext _context;
        private readonly IWebHostEnvironment _env;

        public AnimalsController(FaunaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ------------------ INDEX (список + фильтрация) ------------------
        public IActionResult Index(
            string? searchName,
            List<string>? speciesNames, // ✅ фильтр теперь по названиям видов
            string? gender,
            string? health,
            DateTime? birthFrom,
            DateTime? birthTo)
        {
            var animals = _context.Animals
                .Include(a => a.Species)
                .Include(a => a.Enclosure)
                .AsQueryable();

            // 🔍 Поиск по имени животного
            if (!string.IsNullOrWhiteSpace(searchName))
                animals = animals.Where(a => a.Name.Contains(searchName));

            // ✅ Фильтр по названию вида (несколько можно выбрать)
            if (speciesNames != null && speciesNames.Any())
                animals = animals.Where(a => a.Species != null && speciesNames.Contains(a.Species.Species_name));

            // ⚥ Фильтр по полу
            if (!string.IsNullOrWhiteSpace(gender))
                animals = animals.Where(a => a.Gender == gender);

            // ❤️ Фильтр по здоровью
            if (!string.IsNullOrWhiteSpace(health))
                animals = animals.Where(a => a.Health_status == health);

            // 📅 Фильтр по дате рождения
            if (birthFrom.HasValue)
                animals = animals.Where(a => a.Date_of_birth >= birthFrom.Value);
            if (birthTo.HasValue)
                animals = animals.Where(a => a.Date_of_birth <= birthTo.Value);

            // 📋 Передача данных в View
            ViewBag.SpeciesList = _context.Species.ToList();
            ViewBag.SelectedSpeciesNames = speciesNames ?? new List<string>();
            ViewBag.SelectedName = searchName;
            ViewBag.SelectedGender = gender;
            ViewBag.SelectedHealth = health;
            ViewBag.BirthFrom = birthFrom?.ToString("yyyy-MM-dd");
            ViewBag.BirthTo = birthTo?.ToString("yyyy-MM-dd");

            // 📦 Отображение результата
            return View(animals.OrderBy(a => a.Name).ToList());
        }


        // ------------------ CREATE ------------------
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SpeciesList = _context.Species.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Animals animal, IFormFile? imageFile)
        {
            ViewBag.SpeciesList = _context.Species.ToList();

            if (string.IsNullOrWhiteSpace(animal.Name))
                ModelState.AddModelError("Name", "Введите имя животного");

            if (string.IsNullOrWhiteSpace(animal.Gender))
                ModelState.AddModelError("Gender", "Выберите пол животного");

            if (animal.Species_ID == null)
                ModelState.AddModelError("Species_ID", "Выберите вид животного");

            if (!ModelState.IsValid)
                return View(animal);

            // 🖼 Загрузка изображения
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "animals");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                animal.ImageUrl = $"/images/animals/{fileName}";
            }

            _context.Animals.Add(animal);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // ------------------ EDIT ------------------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var animal = _context.Animals
                .Include(a => a.Species)
                .FirstOrDefault(a => a.Animal_ID == id);

            if (animal == null)
                return NotFound();

            ViewBag.SpeciesList = _context.Species.ToList();
            return View(animal);
        }

        [HttpPost]
        public IActionResult Edit(Animals animal, IFormFile? imageFile)
        {
            var existingAnimal = _context.Animals.Find(animal.Animal_ID);
            if (existingAnimal == null)
                return NotFound();

            // 🖼 Обновление изображения
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/animals");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                existingAnimal.ImageUrl = $"/images/animals/{fileName}";
            }

            // 📝 Обновление данных
            existingAnimal.Name = animal.Name;
            existingAnimal.Gender = animal.Gender;
            existingAnimal.Date_of_birth = animal.Date_of_birth;
            existingAnimal.Health_status = animal.Health_status;
            existingAnimal.Species_ID = animal.Species_ID;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        // ------------------ DELETE ------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var animal = _context.Animals
                .Include(a => a.Species)
                .FirstOrDefault(a => a.Animal_ID == id);

            if (animal == null)
                return NotFound();

            return View(animal);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int Animal_ID)
        {
            var animal = _context.Animals.Find(Animal_ID);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
