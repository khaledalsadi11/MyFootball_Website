using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class AgeGroup : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Age group name is required")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Minimum age must be between 1 and 100")]
        public int MinAge { get; set; }

        [Range(1, 100, ErrorMessage = "Maximum age must be between 1 and 100")]
        public int MaxAge { get; set; }

        // Navigation Property
        public ICollection<Team> Teams { get; set; }
            = new List<Team>();

        // Custom Validation
        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (MinAge > MaxAge)
            {
                yield return new ValidationResult(
                    "Minimum age cannot be greater than maximum age",
                    new[] { nameof(MinAge), nameof(MaxAge) }
                );
            }
        }
    }
}