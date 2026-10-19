using FluentValidation;
using EasyClinic.API.DTOs.AppointmentStatus;

namespace EasyClinic.API.Validators
{
    public class CreateAppointmentStatusDTOValidator : AbstractValidator<CreateAppointmentStatusDTO>
    {
        public CreateAppointmentStatusDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("AppointmentStatus name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("AppointmentStatus description is required.")
                .Length(2, 500).WithMessage("description must be between 2 and 500 characters.");
        }
    }
}
