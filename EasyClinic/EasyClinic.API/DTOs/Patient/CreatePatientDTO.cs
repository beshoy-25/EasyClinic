using EasyClinic.Domain.Entities;

namespace EasyClinic.API.DTOs.Patient
{
    public class CreatePatientDTO
    {        
        public string Name { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MedicalHistory { get; set; } = string.Empty;

    }
}
