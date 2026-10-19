using EasyClinic.API.DTOs.DoctorService;
using FluentValidation;

namespace EasyClinic.API.Validators
{
    public class CreateDoctorServiceDTOValidator : AbstractValidator<CreateDoctorServiceDTO>
    {
        public CreateDoctorServiceDTOValidator()
        {
            RuleFor(x => x.MedicalServiceId)
               .NotEmpty().WithMessage("Medical Service Id is required")
               .GreaterThan(1).WithMessage("ID must be valid positive number");
            RuleFor(x => x.DoctorId)
               .NotEmpty().WithMessage("Doctor Id is required")
               .GreaterThan(1).WithMessage("ID must be valid positive number");
        }
    }
}
