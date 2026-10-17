
using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin,Coach")]
    public class TrainingSessionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainingSessionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private void LoadTeams(int? selectedId = null)
        {
            var teams = _unitOfWork.teams.GetAll("Club");

            ViewBag.Teams = new SelectList(
                teams.Select(x => new
                {
                    x.Id,
                    Name = $"{x.Club?.Name} - {x.Name}"
                }),
                "Id",
                "Name",
                selectedId);
        }

        private void ValidateSession(
            TrainingSession model,
            int? excludeId = null)
        {
            model.Title = (model.Title ?? "").Trim();

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError(
                    nameof(TrainingSession.Title),
                    "Session title is required");
            }

            if (!_unitOfWork.teams.Any(
                x => x.Id == model.TeamId))
            {
                ModelState.AddModelError(
                    nameof(TrainingSession.TeamId),
                    "Please select a valid team");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(
                    nameof(TrainingSession.EndTime),
                    "End time must be after start time");
            }

            // Prevent overlapping sessions for the same team.
            bool overlap = _unitOfWork.trainingSessions.Any(x =>
                x.TeamId == model.TeamId &&
                x.StartTime < model.EndTime &&
                model.StartTime < x.EndTime &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (overlap)
            {
                ModelState.AddModelError(
                    nameof(TrainingSession.StartTime),
                    "This team already has a session during this time");
            }
        }

        // =========================
        // INDEX
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            var sessions = _unitOfWork.trainingSessions
                .GetAll("Team.Club");

            return View(sessions);
        }

        // =========================
        // DETAILS
        // =========================
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions.FindById(
                id, "Team.Club", "Attendances");

            if (session == null)
                return NotFound();

            return View(session);
        }

        // =========================
        // CREATE GET
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            LoadTeams();

            return View(new TrainingSession
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            });
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Title,StartTime,EndTime,Location,TeamId")]
            TrainingSession model)
        {
            ValidateSession(model);

            if (!ModelState.IsValid)
            {
                LoadTeams(model.TeamId);
                return View(model);
            }

            _unitOfWork.trainingSessions.AddOne(model);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create training session.");

                LoadTeams(model.TeamId);
                return View(model);
            }

            TempData["s"] = "Training session created successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions
                .SelectOne(x => x.Id == id);

            if (session == null)
                return NotFound();

            LoadTeams(session.TeamId);

            return View(session);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Title,StartTime,EndTime,Location,TeamId")]
            TrainingSession model)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            var session = _unitOfWork.trainingSessions
                .SelectOne(x => x.Id == id);

            if (session == null)
                return NotFound();

            ValidateSession(model, id);

            if (session.TeamId != model.TeamId &&
                _unitOfWork.attendances.Any(
                    x => x.TrainingSessionId == id))
            {
                ModelState.AddModelError(
                    nameof(TrainingSession.TeamId),
                    "Cannot change team while attendance records exist");
            }

            if (!ModelState.IsValid)
            {
                LoadTeams(model.TeamId);
                return View(model);
            }

            session.Title = model.Title;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;
            session.Location = model.Location;
            session.TeamId = model.TeamId;

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update training session.");

                LoadTeams(model.TeamId);
                return View(model);
            }

            TempData["s"] = "Training session updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE GET
        // =========================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions.FindById(
                id, "Team.Club");

            if (session == null)
                return NotFound();

            return View(session);
        }

        // =========================
        // DELETE POST
        // =========================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id <= 0)
                return NotFound();

            var session = _unitOfWork.trainingSessions
                .SelectOne(x => x.Id == id);

            if (session == null)
                return NotFound();

            if (_unitOfWork.attendances.Any(
                x => x.TrainingSessionId == id))
            {
                TempData["errorData"] =
                    "Cannot delete a session with attendance records.";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.trainingSessions.RemoveOne(session);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                TempData["errorData"] =
                    "Unable to delete training session.";

                return RedirectToAction(nameof(Index));
            }

            TempData["s"] = "Training session deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
