using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Football.Models
{
    public class Club
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Coach? Coach { get; set; }

        public ICollection<Player> Players { get; set; }
            = new List<Player>();

        [Required]
        public int LeagueId { get; set; }//fk
        
        public League ? League { get; set; } 
    }
}
