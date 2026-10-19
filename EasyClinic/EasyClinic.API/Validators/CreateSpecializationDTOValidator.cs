using FluentValidation;
using EasyClinic.API.DTOs.Specializations;

namespace EasyClinic.API.Validators
{
    public class CreateSpecializationDTOValidator : AbstractValidator<CreateSpecializationDTO>
    {
        public CreateSpecializationDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Specialization name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
        }
    }
}