using EasyClinic.Domain.Abstractions;

namespace EasyClinic.Domain.Entities
{
    public class Specialization : Entity
    {
        private Specialization(){}
        public string Name { get; private set; } = string.Empty;
        //Navigation Property
        public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();

        public static Specialization Create(string name) => new() { Name = name };
        public void Rename(string name) => Name = name;

    }
}
