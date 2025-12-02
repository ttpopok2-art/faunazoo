namespace WebApplication8.Models
{
    public class Tickets
    {
        public int Ticket_ID { get; set; }
        public string Ticket_type { get; set; }
        public decimal Price { get; set; }
        public DateTime Sale_date { get; set; }

        public int Visitor_ID { get; set; }
        public Visitors Visitor { get; set; }
    }
}
