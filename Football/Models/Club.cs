using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public class Club
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Club name is required")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Club name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public ICollection<Coach> Coaches { get; set; }
            = new List<Coach>();

        public ICollection<Player> Players { get; set; }
            = new List<Player>();

        public ICollection<Team> Teams { get; set; }
            = new List<Team>();
    }
}