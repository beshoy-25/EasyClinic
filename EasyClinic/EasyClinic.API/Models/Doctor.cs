namespace EasyClinic.API.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        //Forign Key
        public int SpecializationId { get; set; }
        //Navigation Property (الطرف التانى من العلاقه)
        public Specialization Specialization { get; set; } = null!;
        // Navigation Property for another Reelation ship with (DoctorServices)
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();


    }
}
