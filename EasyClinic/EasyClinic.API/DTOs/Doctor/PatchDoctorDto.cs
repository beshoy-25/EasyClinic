namespace EasyClinic.API.DTOs.Doctor
{
    public class PatchDoctorDto
    {
        public string? FullName { get; set; } 
        public string? PhoneNumber { get; set; } 
        public string? Email { get; set; }
        public int? SpecializationId { get; set; }
    }
}
