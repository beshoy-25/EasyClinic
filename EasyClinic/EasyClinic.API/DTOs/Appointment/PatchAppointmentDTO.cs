namespace EasyClinic.API.DTOs.Appointment
{
    public class PatchAppointmentDTO
    {
        public int? PatientId { get; set; }
        public int? DoctorServiceId { get; set; }
        public int? AppointmentStatusId { get; set; }
        public DateTimeOffset? AppointmentDate { get; set; }
        public string? Notes { get; set; }
    }
}