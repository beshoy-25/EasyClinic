using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Errors;

public static class PatientErrors
{
    public static readonly Error DateOfBirthInTheFuture =
        Error.Validation("Patient.DateOfBirthInTheFuture", "Date of birth cannot be in the future.");

    public static Error NotFound(int id) =>
        Error.NotFound("Patient.NotFound", $"Patient with id {id} was not found.");

    public static readonly Error HasAppointments =
        Error.Conflict("Patient.HasAppointments", "Cannot delete a patient who has appointments.");
}