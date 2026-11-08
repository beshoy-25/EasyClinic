using EasyClinic.Domain.Abstractions;
namespace EasyClinic.Domain.Errors;

public static class MedicalServiceErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("MedicalService.NotFound", $"MedicalService with id {id} was not found.");

    public static readonly Error InvalidPrice =
        Error.Validation("MedicalService.InvalidPrice", "Price must be greater than zero.");

    public static readonly Error InvalidDuration =
        Error.Validation("MedicalService.InvalidDuration", "Duration must be greater than zero.");

    public static readonly Error HasDoctors =
        Error.Conflict("MedicalService.HasDoctors", "Cannot delete a medical service that has doctors linked to it.");
}