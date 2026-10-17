
using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoachesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoachesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================
        // HELPERS
        // =========================

        private void LoadDropdowns(
            int? clubId = null,
            int? nationalityId = null,
            int? teamId = null,
            int? coachId = null)
        {
            ViewBag.Clubs = new SelectList(
                _unitOfWork.clubs.GetAll(),
                "Id", "Name", clubId);
            ViewBag.Teams = new SelectList(
                _unitOfWork.teams.GetAll()
                    .Where(x => clubId.HasValue &&
                                x.ClubId == clubId.Value &&
                                (x.CoachId == null ||
                                 (coachId.HasValue && x.CoachId == coachId.Value))),
                "Id", "Name", teamId);

            ViewBag.Nationalities = new SelectList(
                _unitOfWork.nationalites.GetAll(),
                "Id", "Country", nationalityId);
        }

        private void ValidateCoach(Coach model)
        {
            model.Name = (model.Name ?? "").Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(Coach.Name),
                    "Coach name is required");
            }

            if (!_unitOfWork.clubs.Any(
                x => x.Id == model.ClubId))
            {
                ModelState.AddModelError(
                    nameof(Coach.ClubId),
                    "Please select a valid club");
            }

            if (!_unitOfWork.nationalites.Any(
                x => x.Id == model.NationalityId))
            {
                ModelState.AddModelError(
                    nameof(Coach.NationalityId),
                    "Please select a valid nationality");
            }
        }

        // =========================
        // INDEX
        // =========================

        [HttpGet]
        public IActionResult Index()
        {
            var coaches = _unitOfWork.coaches.GetAll(
                "Club", "Nationality", "Teams");

            return View(coaches);
        }

        // =========================
        // DETAILS
        // =========================

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var coach = _unitOfWork.coaches.FindById(
                id, "Club", "Nationality", "Teams");

            if (coach == null)
                return NotFound();

            return View(coach);
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

            return View(new Coach
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
            [Bind("Name,ClubId,NationalityId")] Coach model,
            int? SelectedTeamId)
        {
            ValidateCoach(model);

            Team? selectedTeam = null;
            if (SelectedTeamId.HasValue)
            {
                selectedTeam = _unitOfWork.teams.SelectOne(
                    t => t.Id == SelectedTeamId.Value);

                if (selectedTeam == null ||
                    selectedTeam.ClubId != model.ClubId ||
                    selectedTeam.CoachId.HasValue)
                {
                    ModelState.AddModelError(nameof(SelectedTeamId),
                        "Select an available team from the selected club.");
                }
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.NationalityId,
                    SelectedTeamId);

                return View(model);
            }

            if (selectedTeam != null)
                model.Teams.Add(selectedTeam);

            _unitOfWork.coaches.AddOne(model);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create coach. Check related records.");

                LoadDropdowns(
                    model.ClubId,
                    model.NationalityId,
                    SelectedTeamId);

                return View(model);
            }

            TempData["s"] = "Coach created successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetAvailableTeams(int clubId, int? coachId = null)
        {
            var teams = _unitOfWork.teams.GetAll()
                .Where(t => t.ClubId == clubId &&
                    (t.CoachId == null || (coachId.HasValue && t.CoachId == coachId.Value)))
                .Select(t => new { id = t.Id, name = t.Name });

            return Json(teams);
        }

        // =========================
        // EDIT GET
        // =========================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var coach = _unitOfWork.coaches
                .SelectOne(x => x.Id == id);

            if (coach == null)
                return NotFound();

            var currentTeamId = _unitOfWork.teams.GetAll()
                .Where(t => t.CoachId == id)
                .OrderBy(t => t.Id)
                .Select(t => (int?)t.Id)
                .FirstOrDefault();

            LoadDropdowns(
                coach.ClubId,
                coach.NationalityId,
                currentTeamId,
                id);

            return View(coach);
        }

        // =========================
        // EDIT POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Name,ClubId,NationalityId")] Coach model,
            int? SelectedTeamId)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            var coach = _unitOfWork.coaches
                .SelectOne(x => x.Id == id);

            if (coach == null)
                return NotFound();

            ValidateCoach(model);

            var currentTeams = _unitOfWork.teams.GetAll()
                .Where(t => t.CoachId == id).OrderBy(t => t.Id).ToList();
            var currentTeamId = currentTeams.Select(t => (int?)t.Id).FirstOrDefault();
            var teamChanged = currentTeamId != SelectedTeamId;
            Team? selectedTeam = null;

            if (currentTeams.Count > 1 && teamChanged)
            {
                ModelState.AddModelError(nameof(SelectedTeamId),
                    "This coach has multiple teams. Reassign them from team management first.");
            }

            if (SelectedTeamId.HasValue)
            {
                selectedTeam = _unitOfWork.teams.SelectOne(t => t.Id == SelectedTeamId.Value);
                if (selectedTeam == null || selectedTeam.ClubId != model.ClubId ||
                    (selectedTeam.CoachId.HasValue && selectedTeam.CoachId != id))
                {
                    ModelState.AddModelError(nameof(SelectedTeamId),
                        "Select an available team from the selected club.");
                }
            }

            // Prevent moving coach to another club
            // while assigned to teams.
            if (coach.ClubId != model.ClubId &&
                _unitOfWork.teams.Any(x => x.CoachId == id))
            {
                ModelState.AddModelError(
                    nameof(Coach.ClubId),
                    "Reassign the coach's teams before changing club.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.NationalityId,
                    SelectedTeamId,
                    id);

                return View(model);
            }

            coach.Name = model.Name;
            coach.ClubId = model.ClubId;
            coach.NationalityId = model.NationalityId;

            if (teamChanged)
            {
                foreach (var team in currentTeams)
                    team.CoachId = null;
                if (selectedTeam != null)
                    selectedTeam.CoachId = id;
            }

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update coach. Check related records.");

                // Restore tracked assignments before rebuilding the dropdown.
                if (teamChanged)
                {
                    if (selectedTeam != null) selectedTeam.CoachId = null;
                    foreach (var team in currentTeams) team.CoachId = id;
                }

                LoadDropdowns(
                    model.ClubId,
                    model.NationalityId,
                    SelectedTeamId,
                    id);

                return View(model);
            }

            TempData["s"] = "Coach updated successfully";

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

            var coach = _unitOfWork.coaches.FindById(
                id, "Club", "Nationality", "Teams");

            if (coach == null)
                return NotFound();

            return View(coach);
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

            var coach = _unitOfWork.coaches
                .SelectOne(x => x.Id == id);

            if (coach == null)
                return NotFound();

            if (_unitOfWork.teams.Any(x => x.CoachId == id))
            {
                TempData["errorData"] =
                    "Cannot delete a coach assigned to teams.";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.coaches.RemoveOne(coach);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                TempData["errorData"] =
                    "Unable to delete coach because related records exist.";

                return RedirectToAction(nameof(Index));
            }

            TempData["s"] = "Coach deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
