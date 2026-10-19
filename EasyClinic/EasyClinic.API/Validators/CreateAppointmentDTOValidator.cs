using EasyClinic.API.DTOs.Appointment;
using EasyClinic.API.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace EasyClinic.API.Validators
{
    public class CreateAppointmentDTOValidator : AbstractValidator<CreateAppointmentDTO>
    {
        private readonly ClinicSettings _settings;

        public CreateAppointmentDTOValidator(IOptions<ClinicSettings> options)
        {
            _settings = options.Value;

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient Id is required")
                .GreaterThanOrEqualTo(1).WithMessage("ID must be valid positive number");

            RuleFor(x => x.DoctorServiceId)
                .NotEmpty().WithMessage("DoctorService Id is required")
                .GreaterThanOrEqualTo(1).WithMessage("ID must be valid positive number");

            RuleFor(x => x.AppointmentStatusId)
                .NotEmpty().WithMessage("AppointmentStatus Id is required")
                .GreaterThanOrEqualTo(1).WithMessage("ID must be valid positive number");

            RuleFor(x => x.AppointmentDate)
            .NotEmpty()
             .Must(BeWithinWorkingHours)
            .WithMessage($"Appointments must be between {_settings.OpeningHour}:00 and {_settings.ClosingHour}:00.")            
            .Must(BeInTheFuture).WithMessage("Appointments must be in the future.");

            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("You will reach the maximum length for notes .Maximum length for Notes is 500.");
        }
        private bool BeInTheFuture(DateTimeOffset datetime)
            => datetime > DateTimeOffset.Now;

        private bool BeWithinWorkingHours (DateTimeOffset datetime)
            => datetime.Hour >= _settings.OpeningHour && datetime.Hour < _settings.ClosingHour;

    }
}
