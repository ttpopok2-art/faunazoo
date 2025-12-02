using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication8.Models
{
    public class Species
    {
        public int Species_ID { get; set; }
        public string Species_name { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsRare { get; set; }
        public bool IsEndangered { get; set; }

        [ValidateNever]
        public ICollection<Animals>? Animals { get; set; }
    }
}
