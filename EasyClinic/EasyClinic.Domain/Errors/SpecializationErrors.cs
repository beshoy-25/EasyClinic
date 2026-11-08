using EasyClinic.Domain.Abstractions;
namespace EasyClinic.Domain.Errors;

public static class SpecializationErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Specialization.NotFound", $"Specialization with id {id} was not found.");

    public static readonly Error HasDoctors =
        Error.Conflict("Specialization.HasDoctors", "Cannot delete a specialization that has doctors linked to it.");
    // في SpecializationErrors
    public static readonly Error NameAlreadyExists =
        Error.Conflict("Specialization.NameAlreadyExists", "A specialization with this name already exists.");
}