
using Football.Models;
using System.ComponentModel.DataAnnotations;

namespace Football.ViewModels
{
    public class AttendanceFormViewModel
    {
        public int TrainingSessionId { get; set; }

        public string SessionTitle { get; set; } = "";

        public string TeamName { get; set; } = "";

        public List<PlayerAttendanceViewModel> Players { get; set; }
            = new();
    }

    public class PlayerAttendanceViewModel
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; } = "";

        public int ShirtNumber { get; set; }

        public AttendanceStatus Status { get; set; }
            = AttendanceStatus.Present;

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
