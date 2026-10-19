namespace EasyClinic.API.DTOs.DoctorService
{
    public class DoctorServiceDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int MedicalServiceId { get; set; }
        public string MedicalServiceName { get; set; } = string.Empty;


    }
}
