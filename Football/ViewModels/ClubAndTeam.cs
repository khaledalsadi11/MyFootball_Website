using Football.Models;

namespace Football.ViewModels
{
    public class ClubAndTeam
    {
        IEnumerable<Club> Clubs { get; set; }
        IEnumerable<Team> Team { get; set; }
        Coach Coach { get; set; } = new();


    }
}
