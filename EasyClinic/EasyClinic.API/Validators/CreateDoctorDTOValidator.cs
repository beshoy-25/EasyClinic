using EasyClinic.API.DTOs.Doctor;
using FluentValidation;

namespace EasyClinic.API.Validators
{
    public class CreateDoctorDTOValidator :AbstractValidator<CreateDoctorDTO>
    {
        public CreateDoctorDTOValidator()
        {
            RuleFor(x => x.FullName)  
                .NotEmpty().WithMessage("Doctor name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Doctor Phone Number is required.")
                .Matches(@"^01\d{9}$").WithMessage("Phone Number must br '01' followed by 5 digits \'01XXXXXXXXX\'");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Doctor Email is required.")
                .Matches("^+@+.com$").WithMessage("Email must be {yourname}@{orgainze like (gmail,microsoft)}.com like \'Name@gmail.com\'");
            RuleFor(x => x.SpecializationId)
                .NotEmpty().WithMessage("Specialization Id is required")
                .GreaterThan(1).WithMessage("ID must be valid positive number");

        }
    }
}
