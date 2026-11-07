using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Errors;
using EasyClinic.Domain.Events;

namespace EasyClinic.Domain.Entities
{
    public class Appointment : Entity
    {

        // شيل السطر ده، الـ Id بقى في Entity:
        // public int Id { get; private set; }
        public int PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;
        public int DoctorServiceId { get; private set; }
        public DoctorService DoctorService { get; private set; } = null!;
        public int AppointmentStatusId { get; private set; }
        public AppointmentStatus AppointmentStatus { get; private set; } = null!;
        public DateTimeOffset AppointmentDate { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string? Notes { get; private set; }

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
            var schedule = ValidateSchedule(appointmentDate, now, openingHour, closingHour);
            if (schedule.IsFailure)
                return Result.Failure<Appointment>(schedule.Error);

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorServiceId = doctorServiceId,
                AppointmentStatusId = appointmentStatusId,
                AppointmentDate = appointmentDate,
                TotalPrice = servicePrice,
                Notes = notes
            };

            appointment.RaiseDomainEvent(new AppointmentCreatedEvent(appointment));

            return Result.Success(appointment);
        }
        public Result Reschedule(DateTimeOffset newDate, DateTimeOffset now, int openingHour, int closingHour)
        {
            var schedule = ValidateSchedule(newDate, now, openingHour, closingHour);
            if (schedule.IsFailure)
                return schedule;

            AppointmentDate = newDate;

            var oldDate = AppointmentDate;
            AppointmentDate = newDate;
            RaiseDomainEvent(new AppointmentRescheduledEvent(this, oldDate));
            return Result.Success();
        }

        // السعر بيتثبّت هنا بس، وده اللي بيحمي الـ Price Snapshot
        public void ChangeDoctorService(int doctorServiceId, decimal servicePrice)
        {
            DoctorServiceId = doctorServiceId;
            TotalPrice = servicePrice;
        }

        public void ChangePatient(int patientId) => PatientId = patientId;
        public void ChangeStatus(int newStatusId)
        {
            if (newStatusId == AppointmentStatusId)
                return;

            var oldStatusId = AppointmentStatusId;
            AppointmentStatusId = newStatusId;
            RaiseDomainEvent(new AppointmentStatusChangedEvent(this, oldStatusId));
        }
        public void UpdateNotes(string? notes) => Notes = notes;

        private static Result ValidateSchedule(DateTimeOffset date, DateTimeOffset now, int openingHour, int closingHour)
        {
            if (date <= now)
                return Result.Failure(AppointmentErrors.InThePast);

            if (date.Hour < openingHour || date.Hour >= closingHour)
                return Result.Failure(AppointmentErrors.OutsideWorkingHours(openingHour, closingHour));

            return Result.Success();
        }
    }
}
