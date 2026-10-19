namespace EasyClinic.API.DTOs.Doctor
{
    public class CreateDoctorDTO
    {        
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int SpecializationId { get; set; }        
    }
}
