using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Entities
{
    public class AppointmentStatus : Entity
    {
        private AppointmentStatus(){}
        public string Name { get; private set; } = string.Empty;    
        public string Description { get; private set; } = string.Empty;

        public static AppointmentStatus Create(string name, string description) =>
            new() { Name = name, Description = description };

        public void Update(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
