namespace EasyClinic.API.DTOs.Appointment
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public int DoctorServiceId { get; set; }
        public string DoctorName { get; set; } = string.Empty ;        
        public string MedicalServiceName { get; set; } = string.Empty ;        

        public int AppointmentStatusId { get; set; }
        public string AppointmentStatusName { get; set; } = string.Empty;       

        public DateTimeOffset AppointmentDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }

    }
}
