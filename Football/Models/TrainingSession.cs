
using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class TrainingSession : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a team")]
        public int TeamId { get; set; }

        public Team? Team { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
            = new List<Attendance>();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be after start time",
                    new[] { nameof(EndTime) });
            }
        }
    }
}
