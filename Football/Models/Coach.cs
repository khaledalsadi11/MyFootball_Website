using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Models
{
    public class Coach
    {
        public int Id { get; set; }

        public int ShirtNumber { get; set; }

        public string Name { get; set; }


        public int ClubId { get; set; }          // FK

        public Club Club { get; set; }

        public int NationalityId { get; set; }   // FK


        public Nationality Nationality { get; set; }
    }
}
