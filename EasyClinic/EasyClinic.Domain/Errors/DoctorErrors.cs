using EasyClinic.Domain.Abstractions;
namespace EasyClinic.Domain.Errors;

public static class DoctorErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Doctor.NotFound", $"Doctor with id {id} was not found.");

    public static Error SpecializationNotFound(int id) =>
        Error.Validation("Doctor.SpecializationNotFound", $"Specialization with id {id} does not exist.");

    public static readonly Error HasServices =
        Error.Conflict("Doctor.HasServices", "Cannot delete a doctor who has services linked to them.");
}