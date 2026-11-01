using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Errors;

namespace EasyClinic.Domain.Entities
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

        public static Result<Appointment> Create(
             int patientId,
             int doctorServiceId,
             int appointmentStatusId,
             DateTimeOffset appointmentDate,
             decimal servicePrice,
             string? notes,
             DateTimeOffset now,
             int openingHour,
             int closingHour)
        {
            if (appointmentDate <= now)
                return Result.Failure<Appointment>(AppointmentErrors.InThePast);

            if (appointmentDate.Hour < openingHour || appointmentDate.Hour >= closingHour)
                return Result.Failure<Appointment>(
                    AppointmentErrors.OutsideWorkingHours(openingHour, closingHour));

            return Result.Success(new Appointment
            {
                PatientId = patientId,
                DoctorServiceId = doctorServiceId,
                AppointmentStatusId = appointmentStatusId,
                AppointmentDate = appointmentDate,
                TotalPrice = servicePrice,
                Notes = notes
            });
        }

    }
}
