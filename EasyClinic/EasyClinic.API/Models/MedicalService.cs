namespace EasyClinic.API.Models
{
    public class MedicalService
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();
    }
}
