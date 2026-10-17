
using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class Coach
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a club")]
        public int ClubId { get; set; }

        public Club? Club { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a nationality")]
        public int NationalityId { get; set; }

        public Nationality? Nationality { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
