using System.ComponentModel.DataAnnotations;

namespace EasyClinic.API.Options
{
    public class CorsSettings
    {
        public const string SectionName = "Cors";
        public const string PolicyName = "FrontendPolicy";

        [Required, MinLength(1)]
        public string[] AllowedOrigins { get; set; } = [];
    }
}