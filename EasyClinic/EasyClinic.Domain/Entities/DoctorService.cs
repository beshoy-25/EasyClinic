using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Entities
{
    public class DoctorService :Entity
    {
        private DoctorService(){}
        public int DoctorId { get; private set; }
        public Doctor Doctor { get; private set; } = null!;
        public int MedicalServiceId { get; private set; }
        public MedicalService MedicalService { get; private set; } = null!;


        public static DoctorService Create(int doctorId, int medicalServiceId) =>
       new() { DoctorId = doctorId, MedicalServiceId = medicalServiceId };

        public void Update(int doctorId, int medicalServiceId)
        {
            DoctorId = doctorId;
            MedicalServiceId = medicalServiceId;
        }

    }
}
