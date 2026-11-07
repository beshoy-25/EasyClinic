using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Errors;

namespace EasyClinic.Domain.Entities;

public enum Gender { Male, Female }

public class Patient : Entity
{
    private Patient() { }

    public string Name { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string MedicalHistory { get; private set; } = string.Empty;

    public static Result<Patient> Create(
        string name, DateOnly dateOfBirth, Gender gender,
        string phoneNumber, string medicalHistory, DateOnly today)
    {
        if (dateOfBirth > today)
            return Result.Failure<Patient>(PatientErrors.DateOfBirthInTheFuture);

        return Result.Success(new Patient
        {
            Name = name,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            PhoneNumber = phoneNumber,
            MedicalHistory = medicalHistory
        });
    }

    public Result Update(
        string name, DateOnly dateOfBirth, Gender gender,
        string phoneNumber, string medicalHistory, DateOnly today)
    {
        if (dateOfBirth > today)
            return Result.Failure(PatientErrors.DateOfBirthInTheFuture);

        Name = name;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        PhoneNumber = phoneNumber;
        MedicalHistory = medicalHistory;
        return Result.Success();
    }
}