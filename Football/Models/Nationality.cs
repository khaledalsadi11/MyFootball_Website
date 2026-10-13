using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Models
{
    public class Nationality
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public ICollection<Player> Players { get; set; }

        public ICollection<Coach> Coaches { get; set; }
    }
}
