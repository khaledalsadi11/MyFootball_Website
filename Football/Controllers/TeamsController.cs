
using System;
using System.Linq;
using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    public class TeamsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================
        // INDEX
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            var teams = _unitOfWork.teams.GetAll(
                "Club", "AgeGroup", "Coach");

            return View(teams);
        }

        // =========================
        // DETAILS
        // =========================
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var team = _unitOfWork.teams.FindById(
                id, "Club", "AgeGroup", "Coach", "Players");

            if (team == null)
                return NotFound();

            return View(team);
        }

        // =========================
        // CREATE GET
        // =========================
        [HttpGet]
        public IActionResult Create(int? clubId)
        {
            if (clubId.HasValue &&
                !_unitOfWork.clubs.Any(x => x.Id == clubId.Value))
            {
                return NotFound();
            }

            LoadDropdowns(clubId);

            return View(new Team
            {
                ClubId = clubId ?? 0
            });
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Name,ClubId,AgeGroupId,CoachId")] Team model)
        {
            ValidateTeam(model);

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.AgeGroupId,
                    model.CoachId);

                return View(model);
            }

            _unitOfWork.teams.AddOne(model);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the team. Check related records.");

                LoadDropdowns(
                    model.ClubId,
                    model.AgeGroupId,
                    model.CoachId);

                return View(model);
            }

            TempData["s"] = "Team created successfully";

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

            var team = _unitOfWork.teams
                .SelectOne(x => x.Id == id);

            if (team == null)
                return NotFound();

            LoadDropdowns(
                team.ClubId,
                team.AgeGroupId,
                team.CoachId);

            return View(team);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Name,ClubId,AgeGroupId,CoachId")] Team model)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            var team = _unitOfWork.teams
                .SelectOne(x => x.Id == id);

            if (team == null)
                return NotFound();

            ValidateTeam(model, id);

            // Prevent moving a team with players
            // to another club.
            if (team.ClubId != model.ClubId &&
                _unitOfWork.players.Any(x => x.TeamId == id))
            {
                ModelState.AddModelError(
                    nameof(Team.ClubId),
                    "Cannot move a team with players to another club.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.AgeGroupId,
                    model.CoachId);

                return View(model);
            }

            team.Name = model.Name;
            team.ClubId = model.ClubId;
            team.AgeGroupId = model.AgeGroupId;
            team.CoachId = model.CoachId;

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the team. Check related records.");

                LoadDropdowns(
                    model.ClubId,
                    model.AgeGroupId,
                    model.CoachId);

                return View(model);
            }

            TempData["s"] = "Team updated successfully";

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

            var team = _unitOfWork.teams.FindById(
                id, "Club", "AgeGroup", "Coach");

            if (team == null)
                return NotFound();

            return View(team);
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

            var team = _unitOfWork.teams
                .SelectOne(x => x.Id == id);

            if (team == null)
                return NotFound();

            bool hasPlayers = _unitOfWork.players
                .Any(x => x.TeamId == id);

            bool hasSessions = _unitOfWork.trainingSessions
                .Any(x => x.TeamId == id);

            if (hasPlayers || hasSessions)
            {
                TempData["errorData"] =
                    "Cannot delete a team with players or training sessions.";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.teams.RemoveOne(team);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                TempData["errorData"] =
                    "Unable to delete the team because related records exist.";

                return RedirectToAction(nameof(Index));
            }

            TempData["s"] = "Team deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // AJAX: COACHES BY CLUB
        // =========================
        [HttpGet]
        public IActionResult CoachesByClub(int clubId)
        {
            if (!_unitOfWork.clubs.Any(x => x.Id == clubId))
                return NotFound();

            var coaches = _unitOfWork.coaches
                .GetAll()
                .Where(x => x.ClubId == clubId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToList();

            return Json(coaches);
        }

        // =========================
        // LOAD DROPDOWNS
        // =========================
        private void LoadDropdowns(
            int? selectedClub = null,
            int? selectedAgeGroup = null,
            int? selectedCoach = null)
        {
            ViewBag.Clubs = new SelectList(
                _unitOfWork.clubs.GetAll(),
                "Id", "Name", selectedClub);

            ViewBag.AgeGroups = new SelectList(
                _unitOfWork.ageGroups.GetAll(),
                "Id", "Name", selectedAgeGroup);

            var coaches = _unitOfWork.coaches
                .GetAll()
                .Where(x => selectedClub.HasValue &&
                            x.ClubId == selectedClub.Value);

            ViewBag.Coaches = new SelectList(
                coaches, "Id", "Name", selectedCoach);
        }

        // =========================
        // VALIDATION
        // =========================
        private void ValidateTeam(
            Team model,
            int? excludeId = null)
        {
            // Navigation property is not submitted.
            ModelState.Remove(nameof(Team.Club));

            model.Name = (model.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(Team.Name),
                    "Team name is required");
            }

            if (!_unitOfWork.clubs.Any(
                x => x.Id == model.ClubId))
            {
                ModelState.AddModelError(
                    nameof(Team.ClubId),
                    "Please select a valid club");
            }

            if (!_unitOfWork.ageGroups.Any(
                x => x.Id == model.AgeGroupId))
            {
                ModelState.AddModelError(
                    nameof(Team.AgeGroupId),
                    "Please select a valid age group");
            }

            if (model.CoachId.HasValue &&
                !_unitOfWork.coaches.Any(x =>
                    x.Id == model.CoachId.Value &&
                    x.ClubId == model.ClubId))
            {
                ModelState.AddModelError(
                    nameof(Team.CoachId),
                    "Select a coach belonging to this club");
            }

            bool duplicate = _unitOfWork.teams.Any(x =>
                x.ClubId == model.ClubId &&
                x.Name.Trim().ToUpper() == model.Name.ToUpper() &&
                (!excludeId.HasValue ||
                 x.Id != excludeId.Value));

            if (duplicate)
            {
                ModelState.AddModelError(
                    nameof(Team.Name),
                    "A team with this name already exists in this club");
            }
        }
    }
}
