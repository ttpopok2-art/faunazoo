namespace WebApplication8.Models
{
    public class MedicalCheckups
    {
        public int Checkup_ID { get; set; }
        public DateTime Checkup_date { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }

        public int Animal_ID { get; set; }
        public Animals Animal { get; set; }

        public int Employee_ID { get; set; }
        public Employees Employee { get; set; }
    }
}
