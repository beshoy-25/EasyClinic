namespace EasyClinic.API.DTOs.Doctor
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; } = string.Empty;
    }
}
