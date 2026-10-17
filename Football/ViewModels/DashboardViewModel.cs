using System;
using System.Collections.Generic;

namespace Football.ViewModels
{
    public class DashboardViewModel
    {
        public int ClubCount { get; set; }
        public int PlayerCount { get; set; }
        public int CoachCount { get; set; }
        public int TeamCount { get; set; }
        public int AgeGroupCount { get; set; }
        public int TrainingSessionCount { get; set; }
        public int AttendanceRecordCount { get; set; }
        public int UndatedSessionCount { get; set; }
        public List<DashboardSessionViewModel> UpcomingSessions { get; set; }
            = new List<DashboardSessionViewModel>();
    }

    public class DashboardSessionViewModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
    }
}
