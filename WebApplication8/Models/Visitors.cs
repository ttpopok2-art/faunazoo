namespace WebApplication8.Models
{
    public class Visitors
    {
        public int Visitor_ID { get; set; }
        public string Full_name { get; set; }
        public string Contact { get; set; }

        
        public ICollection<Tickets> Tickets { get; set; }
    }
}
