
using Football.Models;
using Football.Repository.IRepository;
using Football.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin,Coach")]
    public class AttendancesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendancesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Attendances
        [HttpGet]
        public IActionResult Index()
        {
            var sessions = _unitOfWork.trainingSessions
                .GetAll("Team.Club", "Attendances");

            return View(sessions);
        }

        [HttpGet]
        public IActionResult Records(int sessionId)
        {
            if (sessionId <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions.FindById(
                sessionId, "Team.Club", "Attendances.Player");
            return session == null ? NotFound() : View(session);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var record = _unitOfWork.attendances.FindById(id, "Player", "TrainingSession");
            return record == null ? NotFound() : View(record);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var record = _unitOfWork.attendances.FindById(id, "Player", "TrainingSession");
            return record == null ? NotFound() : View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Status,Notes")] Attendance model)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            var record = _unitOfWork.attendances.FindById(id, "Player", "TrainingSession");
            if (record == null)
                return NotFound();

            if (!Enum.IsDefined(typeof(AttendanceStatus), model.Status))
                ModelState.AddModelError(nameof(model.Status), "Select a valid attendance status.");

            // Keep the original player and session; only status and notes can change.
            if (!ModelState.IsValid)
            {
                model.PlayerId = record.PlayerId;
                model.Player = record.Player;
                model.TrainingSessionId = record.TrainingSessionId;
                model.TrainingSession = record.TrainingSession;
                return View(model);
            }

            record.Status = model.Status;
            record.Notes = model.Notes;
            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to update this attendance record. Please try again.");
                return View(record);
            }

            TempData["s"] = "Attendance record updated successfully";
            return RedirectToAction(nameof(Records), new { sessionId = record.TrainingSessionId });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var record = _unitOfWork.attendances.FindById(id, "Player", "TrainingSession");
            return record == null ? NotFound() : View(record);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id <= 0)
                return NotFound();

            var record = _unitOfWork.attendances.FindById(id, "Player", "TrainingSession");
            if (record == null)
                return NotFound();

            var sessionId = record.TrainingSessionId;
            _unitOfWork.attendances.RemoveOne(record);
            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete this attendance record. Please try again.");
                return View("Delete", record);
            }

            TempData["s"] = "Attendance record deleted successfully";
            return RedirectToAction(nameof(Records), new { sessionId });
        }

        [HttpGet]
        public IActionResult DeleteAll(int sessionId)
        {
            if (sessionId <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions.FindById(
                sessionId, "Team", "Attendances");
            return session == null ? NotFound() : View(session);
        }

        [HttpPost]
        [ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAllConfirmed(int sessionId, int expectedCount)
        {
            if (sessionId <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions.FindById(
                sessionId, "Team", "Attendances");
            if (session == null)
                return NotFound();

            var records = session.Attendances.ToList();
            if (records.Count == 0)
            {
                TempData["s"] = "This session has no attendance records to delete.";
                return RedirectToAction(nameof(Records), new { sessionId });
            }

            if (!ModelState.IsValid || expectedCount != records.Count)
            {
                ModelState.AddModelError(string.Empty,
                    "The attendance count changed. Review the current count and confirm again.");
                return View("DeleteAll", session);
            }

            _unitOfWork.attendances.RemoveList(records);
            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to delete attendance records. No changes were saved. Please try again.");
                return View("DeleteAll", session);
            }

            TempData["s"] = $"Deleted {records.Count} attendance record(s) from this session.";
            return RedirectToAction(nameof(Records), new { sessionId });
        }

        // GET: Attendances/Create?sessionId=5
        [HttpGet]
        public IActionResult Create(int sessionId)
        {
            if (sessionId <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions
                .FindById(sessionId, "Team");

            if (session == null || session.Team == null)
                return NotFound();

            var players = _unitOfWork.players.GetAll()
                .Where(x => x.TeamId == session.TeamId)
                .OrderBy(x => x.ShirtNumber)
                .ToList();

            var existing = _unitOfWork.attendances.GetAll()
                .Where(x => x.TrainingSessionId == sessionId)
                .ToDictionary(x => x.PlayerId);

            var model = new AttendanceFormViewModel
            {
                TrainingSessionId = session.Id,
                SessionTitle = session.Title,
                TeamName = session.Team.Name,

                Players = players.Select(p =>
                {
                    existing.TryGetValue(p.Id, out var record);

                    return new PlayerAttendanceViewModel
                    {
                        PlayerId = p.Id,
                        PlayerName = p.Name,
                        ShirtNumber = p.ShirtNumber,
                        Status = record?.Status
                            ?? AttendanceStatus.Present,
                        Notes = record?.Notes
                    };
                }).ToList()
            };

            return View(model);
        }

        // POST: Attendances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AttendanceFormViewModel model)
        {
            var session = _unitOfWork.trainingSessions
                .FindById(model.TrainingSessionId, "Team");

            if (session == null || session.Team == null)
                return NotFound();

            var players = _unitOfWork.players.GetAll()
                .Where(x => x.TeamId == session.TeamId)
                .ToList();

            var validIds = players.Select(x => x.Id).ToHashSet();

            // Only players from this session's team are allowed.
            if (model.Players == null ||
                model.Players.Select(x => x.PlayerId).Distinct().Count()
                    != model.Players.Count ||
                model.Players.Count != validIds.Count ||
                model.Players.Any(x => !validIds.Contains(x.PlayerId)))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid player list for this training session.");
            }

            if (model.Players != null &&
                model.Players.Any(x =>
                    !Enum.IsDefined(typeof(AttendanceStatus), x.Status)))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid attendance status.");
            }

            if (!ModelState.IsValid)
            {
                model.SessionTitle = session.Title;
                model.TeamName = session.Team.Name;

                if (model.Players != null)
                {
                    foreach (var row in model.Players)
                    {
                        var player = players.FirstOrDefault(
                            x => x.Id == row.PlayerId);

                        row.PlayerName = player?.Name ?? "";
                        row.ShirtNumber = player?.ShirtNumber ?? 0;
                    }
                }

                return View(model);
            }

            var existing = _unitOfWork.attendances.GetAll()
                .Where(x =>
                    x.TrainingSessionId == model.TrainingSessionId)
                .ToDictionary(x => x.PlayerId);

            foreach (var row in model.Players)
            {
                if (existing.TryGetValue(row.PlayerId, out var attendance))
                {
                    attendance.Status = row.Status;
                    attendance.Notes = row.Notes;
                }
                else
                {
                    _unitOfWork.attendances.AddOne(new Attendance
                    {
                        PlayerId = row.PlayerId,
                        TrainingSessionId = model.TrainingSessionId,
                        Status = row.Status,
                        Notes = row.Notes
                    });
                }
            }

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to save attendance records.");

                model.SessionTitle = session.Title;
                model.TeamName = session.Team.Name;

                foreach (var row in model.Players)
                {
                    var player = players.First(x => x.Id == row.PlayerId);
                    row.PlayerName = player.Name;
                    row.ShirtNumber = player.ShirtNumber;
                }

                return View(model);
            }

            TempData["s"] = "Attendance saved successfully";

            return RedirectToAction(
                "Details",
                "TrainingSessions",
                new { id = model.TrainingSessionId });
        }
    }
}
