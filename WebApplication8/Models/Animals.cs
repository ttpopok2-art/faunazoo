using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication8.Models
{
    public class Animals
    {
        public int Animal_ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? Date_of_birth { get; set; }
        public string Health_status { get; set; }

        public string? ImageUrl { get; set; } // 🔹 путь к фото

        public int? Species_ID { get; set; }

        [ValidateNever]
        public Species? Species { get; set; }

        public int? Enclosure_ID { get; set; }

        [ValidateNever]
        public Enclosures? Enclosure { get; set; }

        [ValidateNever]
        public ICollection<Feeding>? Feeding { get; set; }

        [ValidateNever]
        public ICollection<MedicalCheckups>? MedicalCheckups { get; set; }
    }
}
