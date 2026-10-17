// Adjust this import to the namespace that contains your existing IUnitOfWork.
using Football.Repository.IRepository;
using Football.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Assumes synchronous GetAll(). Materialize before local formatting.
            // Keep calls sequential: repositories may share one EF Core DbContext.
            var clubs = _unitOfWork.clubs.GetAll().ToList();
            var players = _unitOfWork.players.GetAll().ToList();
            var coaches = _unitOfWork.coaches.GetAll().ToList();
            var teams = _unitOfWork.teams.GetAll().ToList();
            var ageGroups = _unitOfWork.ageGroups.GetAll().ToList();
            var sessions = _unitOfWork.trainingSessions.GetAll().ToList();
            var attendances = _unitOfWork.attendances.GetAll().ToList();

            var sessionRows = sessions.Select((session, index) =>
            {
                // Set this ONLY after checking your session model.
                // Example for a DateTime or DateTime? property named SessionDate:
                // DateTime? date = session.SessionDate;
                DateTime? date = null;

                return new DashboardSessionViewModel
                {
                    Title = "Training session " + (index + 1),
                    Date = date
                };
            }).ToList();

            var today = DateTime.Today;
            var model = new DashboardViewModel
            {
                ClubCount = clubs.Count,
                PlayerCount = players.Count,
                CoachCount = coaches.Count,
                TeamCount = teams.Count,
                AgeGroupCount = ageGroups.Count,
                TrainingSessionCount = sessions.Count,
                AttendanceRecordCount = attendances.Count,
                UndatedSessionCount = sessionRows.Count(s => !s.Date.HasValue),
                UpcomingSessions = sessionRows
                    .Where(s => s.Date.HasValue && s.Date.Value >= today)
                    .OrderBy(s => s.Date)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }
    }
}
