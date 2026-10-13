using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Models
{
    public class Club
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string league { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Coach? Coach { get; set; }

        public ICollection<Player> Players { get; set; }
            = new List<Player>();
    }
}
