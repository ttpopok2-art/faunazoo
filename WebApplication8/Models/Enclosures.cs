namespace WebApplication8.Models
{
    public class Enclosures
    {
        public int Enclosure_ID { get; set; }

        public string? Type { get; set; }      
        public string? Size { get; set; }     
        public string? Location { get; set; }  
        public string? ImageUrl { get; set; }  

        public int Capacity { get; set; }

        public ICollection<Animals> Animals { get; set; } = new List<Animals>();
        public ICollection<Employees> Employees { get; set; } = new List<Employees>();
    }
}
