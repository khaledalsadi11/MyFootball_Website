using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class League
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LeagueName { get; set; }
        ICollection<Club> clubs { get; set; }
    }
}
