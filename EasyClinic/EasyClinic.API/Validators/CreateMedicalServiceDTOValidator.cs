using EasyClinic.API.DTOs.MedicalService;
using FluentValidation;

namespace EasyClinic.API.Validators
{
    public class CreateMedicalServiceDTOValidator : AbstractValidator<CreateMedicalServiceDTO>
    {
        public CreateMedicalServiceDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("MedicalService name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
            
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("MedicalService description is required.")
                .Length(2, 500).WithMessage("description must be between 2 and 500 characters.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("MedicalService price is required.")
                .GreaterThan(0).WithMessage("Price must be at least 0.01.");

            RuleFor(x => x.DurationInMinutes)
                .NotEmpty().WithMessage("MedicalService duration In Minutes is required.")
                .InclusiveBetween(1, 60).WithMessage("duration must be between 1 and 60 minuts.");
        }
    }
}
