using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class League
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "League name is required")]
        [StringLength(100)]
        public string LeagueName { get; set; } = string.Empty;
    }
}