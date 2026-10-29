namespace EasyClinic.Domain.Entities
{
    public enum Gender
    {
        Male,
        Female
    }
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string MedicalHistory { get; set; } = string.Empty;

    }
}
