using EasyClinic.Domain.Abstractions;
namespace EasyClinic.Domain.Errors;


public static class AppointmentStatusErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("AppointmentStatus.NotFound", $"AppointmentStatus with id {id} was not found.");

    public static readonly Error InUse =
        Error.Conflict("AppointmentStatus.InUse", "Cannot delete a status that is used by appointments.");
}