using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Entities
{
    public class Doctor :Entity
    {
        private Doctor(){}
        public string FullName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        //Forign Key
        public int SpecializationId { get; private set; }
        //Navigation Property (الطرف التانى من العلاقه)
        public Specialization Specialization { get; private set; } = null!;
        // Navigation Property for another Reelation ship with (DoctorServices)
        public ICollection<DoctorService> DoctorServices { get; private set; } = new List<DoctorService>();



        public static Doctor Create(string fullName, string phoneNumber, string? email, int specializationId) =>
        new()
        {
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            SpecializationId = specializationId
        };

        public void Update(string fullName, string phoneNumber, string? email, int specializationId)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email = email;
            SpecializationId = specializationId;
        }

    }
}
