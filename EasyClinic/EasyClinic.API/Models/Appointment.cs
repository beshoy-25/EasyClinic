namespace EasyClinic.API.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorServiceId { get; set; }
        public DoctorService DoctorService { get; set; } = null!;

        public int AppointmentStatusId { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; } = null!;

        public DateTimeOffset AppointmentDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string?  Notes { get; set; }

    }
}
