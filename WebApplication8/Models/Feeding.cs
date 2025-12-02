namespace WebApplication8.Models
{
    public class Feeding
    {
        public int Feeding_ID { get; set; }
        public string Food_name { get; set; }
        public string Quantity { get; set; }
        public DateTime Feeding_time { get; set; }

        public int Animal_ID { get; set; }
        public Animals Animal { get; set; }
    }
}
