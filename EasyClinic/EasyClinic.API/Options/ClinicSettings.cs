using System.ComponentModel.DataAnnotations;

namespace EasyClinic.API.Options
{
    public class ClinicSettings
    {
        public const string SectionName = "ClinicSettings";

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, 23)]
        public int OpeningHour { get; set; }

        [Range(1, 24)]
        public int ClosingHour { get; set; }

        [Range(1, 500)]
        public int MaxAppointmentsPerDay { get; set; }
    }
}