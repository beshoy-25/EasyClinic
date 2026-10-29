namespace EasyClinic.Domain.Entities
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //Navigation Property
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
