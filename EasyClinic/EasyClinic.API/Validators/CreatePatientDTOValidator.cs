using EasyClinic.API.DTOs.Patient;
using EasyClinic.Domain.Entities;
using FluentValidation;

namespace EasyClinic.API.Validators
{
    public class CreatePatientDTOValidator : AbstractValidator<CreatePatientDTO>
    {
        public CreatePatientDTOValidator()
        {
            RuleFor(x =>x.Name)
                .NotEmpty().WithMessage("Patient name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Doctor Phone Number is required.")
                .Matches(@"^01\d{9}$").WithMessage("Phone Number must br '01' followed by 5 digits \'01XXXXXXXXX\'");

            RuleFor(x => x.MedicalHistory)
                .NotEmpty().WithMessage("Medical History is required")
                .MaximumLength(500).WithMessage("You will reach the maximum length for Medical History .Maximum length is 500.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Patient Date Of Birth is required.");
                

            RuleFor(x => x.Gender).Matches("Male|Female").WithMessage("Gender must be \'Male\' or \'Female\'");

        }
    }
}
