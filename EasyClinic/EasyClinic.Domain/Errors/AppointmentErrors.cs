using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Errors;

public static class AppointmentErrors
{
    public static readonly Error InThePast =
        Error.Validation("Appointment.InThePast",
            "Appointments must be in the future.");

    public static Error OutsideWorkingHours(int openingHour, int closingHour) =>
        Error.Validation("Appointment.OutsideWorkingHours",
            $"Appointments must be between {openingHour}:00 and {closingHour}:00.");

    public static readonly Error DailyLimitReached =
        Error.Conflict("Appointment.DailyLimitReached",
            "The maximum number of appointments for this day has been reached. Please choose another day.");

    public static Error PatientNotFound(int patientId) =>
        Error.Validation("Appointment.PatientNotFound",
            $"Patient with id {patientId} does not exist.");

    public static Error DoctorServiceNotFound(int doctorServiceId) =>
        Error.Validation("Appointment.DoctorServiceNotFound",
            $"DoctorService with id {doctorServiceId} does not exist.");

    public static Error StatusNotFound(int statusId) =>
        Error.Validation("Appointment.StatusNotFound",
            $"AppointmentStatus with id {statusId} does not exist.");
}