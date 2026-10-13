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


        public int ShirtNumber { get; set; }

        [NotNull]
        [MaxLength(20)]
        public string Name { get; set; }

        [NotNull]
        public string Position { get; set; }

        //fk
        public int ClubId {  get; set; }

        // player have one club
        public Club Club { get; set; }

        //fk
       
        public int ? NationalityId { get; set; }

        public Nationality Nationality { get; set; }


    }
}
