namespace EasyClinic.API.Models
{
    public class DoctorService
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int MedicalServiceId { get; set; }
        public MedicalService MedicalService { get; set; } = null!;

    }
}
