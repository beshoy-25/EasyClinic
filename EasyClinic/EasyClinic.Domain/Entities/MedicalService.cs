using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Errors;

namespace EasyClinic.Domain.Entities
{
    public class MedicalService : Entity
    {
        private MedicalService() {}
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int DurationInMinutes { get; private set; }
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();

        public static Result<MedicalService> Create(string name, string description, decimal price, int durationInMinutes)
        {
            var check = Validate(price, durationInMinutes);
            if (check.IsFailure)
                return Result.Failure<MedicalService>(check.Error);

            return Result.Success(new MedicalService
            {
                Name = name,
                Description = description,
                Price = price,
                DurationInMinutes = durationInMinutes
            });
        }

        // Invariants: صح في أي وقت، فبنفحصها في كل Update
        public Result Update(string name, string description, decimal price, int durationInMinutes)
        {
            var check = Validate(price, durationInMinutes);
            if (check.IsFailure)
                return check;

            Name = name;
            Description = description;
            Price = price;
            DurationInMinutes = durationInMinutes;
            return Result.Success();
        }

        private static Result Validate(decimal price, int durationInMinutes)
        {
            if (price <= 0)
                return Result.Failure(MedicalServiceErrors.InvalidPrice);

            if (durationInMinutes <= 0)
                return Result.Failure(MedicalServiceErrors.InvalidDuration);

            return Result.Success();
        }

    }
}
