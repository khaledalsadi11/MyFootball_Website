using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Age Group
        [Range(1, int.MaxValue, ErrorMessage = "Please select an age group")]

        public int AgeGroupId { get; set; }
        public AgeGroup? AgeGroup { get; set; }

        // Coach
        public int? CoachId { get; set; }
        public Coach? Coach { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a club")]

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        // Players
        public ICollection<Player> Players { get; set; }
            = new List<Player>();

        // Training Sessions
        public ICollection<TrainingSession> TrainingSessions { get; set; }
            = new List<TrainingSession>();
    }
}