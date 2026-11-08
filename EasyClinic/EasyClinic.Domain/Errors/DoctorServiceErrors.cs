using EasyClinic.Domain.Abstractions;
namespace EasyClinic.Domain.Errors;

public static class DoctorServiceErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("DoctorService.NotFound", $"DoctorService with id {id} was not found.");

    public static Error DoctorNotFound(int id) =>
        Error.Validation("DoctorService.DoctorNotFound", $"Doctor with id {id} does not exist.");

    public static Error MedicalServiceNotFound(int id) =>
        Error.Validation("DoctorService.MedicalServiceNotFound", $"MedicalService with id {id} does not exist.");

    public static readonly Error AlreadyExists =
        Error.Conflict("DoctorService.AlreadyExists", "This doctor is already linked to this medical service.");

    public static readonly Error HasAppointments =
        Error.Conflict("DoctorService.HasAppointments", "Cannot change or delete a doctor-service link that already has appointments.");
}