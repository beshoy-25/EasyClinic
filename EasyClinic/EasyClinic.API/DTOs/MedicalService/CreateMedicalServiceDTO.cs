namespace EasyClinic.API.DTOs.MedicalService
{
    public class CreateMedicalServiceDTO
    {        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
