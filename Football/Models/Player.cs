using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Football.Models
{
    public class Player
    {
        
        public int Id { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Range(1, 99, ErrorMessage = "Shirt number must be between 1 and 99")]

        public int ShirtNumber { get; set; }

        [Required(ErrorMessage = "Player name is required")]
        [StringLength(100, MinimumLength = 3,
              ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(30, ErrorMessage = "Position is too long")]
        public string Position { get; set; }

        //fk
        [Range(1, int.MaxValue, ErrorMessage = "Please select a club")]

        public int ClubId {  get; set; }

        // player have one club
        public Club? Club { get; set; }

        //fk
        [Range(1, int.MaxValue, ErrorMessage = "Please select a nationality")]

        public int ? NationalityId { get; set; }

        public Nationality? Nationality { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
    = new List<Attendance>();
    }
}
