namespace WebApplication8.Models
{
    public class Employees
    {
        public int Employee_ID { get; set; }
        public string Full_name { get; set; }
        public string Position { get; set; }
        public string Phone_number { get; set; }
        public int Work_experience { get; set; }
        public string? ImageUrl { get; set; }

        public int? Enclosure_ID { get; set; }
        public Enclosures? Enclosure { get; set; }

        public ICollection<MedicalCheckups> MedicalCheckups { get; set; }

        public Employees()
        {
            MedicalCheckups = new List<MedicalCheckups>();
        }
    }
}
