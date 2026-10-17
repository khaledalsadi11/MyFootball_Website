
using System.ComponentModel.DataAnnotations;

namespace Football.Models
{
    public enum AttendanceStatus
    {
        Absent = 0,
        Present = 1,
        Late = 2,
        Excused = 3
    }

    public class Attendance
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int TrainingSessionId { get; set; }
        public TrainingSession? TrainingSession { get; set; }

        public AttendanceStatus Status { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
