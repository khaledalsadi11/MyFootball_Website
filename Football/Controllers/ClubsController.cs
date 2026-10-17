
using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClubsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClubsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================
        // HELPERS
        // =========================

        private void LoadDropdowns(
     int? clubId = null,
     int? teamId = null)
        {

            ViewBag.Teams = new SelectList(
                _unitOfWork.teams.GetAll()
                    .Where(x => clubId.HasValue &&
                                x.ClubId == clubId.Value),
                "Id", "Name", teamId);
        }
        private void ValidateClub(
            Club club,
            int? excludeId = null)
        {
            club.Name = (club.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(club.Name))
            {
                ModelState.AddModelError(
                    nameof(Club.Name),
                    "Club name is required");
            }

            bool duplicate = _unitOfWork.clubs.Any(x =>
                x.Name.Trim().ToUpper() == club.Name.ToUpper() &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (duplicate)
            {
                ModelState.AddModelError(
                    nameof(Club.Name),
                    "Club name already exists");
            }
        }

        private bool HasRelatedRecords(int clubId)
        {
            return
                _unitOfWork.players.Any(x => x.ClubId == clubId) ||
                _unitOfWork.teams.Any(x => x.ClubId == clubId) ||
                _unitOfWork.coaches.Any(x => x.ClubId == clubId);
        }

        // =========================
        // INDEX + SEARCH
        // =========================

        [HttpGet]
        public IActionResult Index(string? search)
        {
            var clubs = _unitOfWork.clubs
                .GetAll()
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();

                clubs = clubs.Where(x =>
                    x.Name.Contains(
                        keyword,
                        StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Search = search;

            return View(clubs.ToList());
        }

        // =========================
        // CREATE GET
        // =========================

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Club());
        }

        // =========================
        // CREATE POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Name")] Club model)
        {
            ValidateClub(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _unitOfWork.clubs.AddOne(model);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to save the club. Check its name.");

                return View(model);
            }

            TempData["s"] = "Club added successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                return NotFound();

            var club = _unitOfWork.clubs
                .SelectOne(x => x.Id == id.Value);

            if (club == null)
                return NotFound();

            return View(club);
        }

        // =========================
        // EDIT POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Name")] Club model)
        {
            if (id != model.Id || id <= 0)
                return BadRequest();

            var existingClub = _unitOfWork.clubs
                .SelectOne(x => x.Id == id);

            if (existingClub == null)
                return NotFound();

            ValidateClub(model, id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            existingClub.Name = model.Name;

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the club. Check its name.");

                return View(model);
            }

            TempData["s"] = "Club updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS GET
        // =========================

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                return NotFound();

            var club = _unitOfWork.clubs.FindById(
                id.Value,
                "Teams",
                "Players.Nationality"
            );

            if (club == null)
                return NotFound();

            return View(club);
        }

        // =========================
        // DELETE GET
        // =========================

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                return NotFound();

            var club = _unitOfWork.clubs.FindById(id.Value);

            if (club == null)
                return NotFound();

            return View(club);
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

            var club = _unitOfWork.clubs
                .SelectOne(x => x.Id == id);

            if (club == null)
                return NotFound();

            if (HasRelatedRecords(id))
            {
                TempData["errorData"] =
                    "Cannot delete a club that has players, teams or coaches.";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.clubs.RemoveOne(club);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                TempData["errorData"] =
                    "Cannot delete this club because related records exist.";

                return RedirectToAction(nameof(Index));
            }

            TempData["s"] = "Club deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
