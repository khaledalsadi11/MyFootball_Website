
using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlayersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Players
        [HttpGet]
        public IActionResult Index()
        {
            var players = _unitOfWork.players.GetAll(
                "Club", "Team", "Nationality");

            return View(players);
        }

        // GET: Players/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var player = _unitOfWork.players.FindById(
                id, "Club", "Team", "Nationality");

            if (player == null)
                return NotFound();

            return View(player);
        }

        // GET: Players/Create
        [HttpGet]
        public IActionResult Create(int? clubId)
        {
            if (clubId.HasValue &&
                !_unitOfWork.clubs.Any(x => x.Id == clubId.Value))
                return NotFound();

            LoadDropdowns(clubId);

            return View(new Player
            {
                ClubId = clubId ?? 0
            });
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Name,ShirtNumber,Position,ClubId,TeamId,NationalityId")]
            Player model)
        {
            ValidatePlayer(model);

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.TeamId,
                    model.NationalityId);

                return View(model);
            }

            model.CreatedOn = DateTime.UtcNow;
            _unitOfWork.players.AddOne(model);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create player. Check related records.");

                LoadDropdowns(
                    model.ClubId,
                    model.TeamId,
                    model.NationalityId);

                return View(model);
            }

            TempData["s"] = "Player created successfully";

            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var player = _unitOfWork.players
                .SelectOne(x => x.Id == id);

            if (player == null)
                return NotFound();

            LoadDropdowns(
                player.ClubId,
                player.TeamId,
                player.NationalityId);

            return View(player);
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Name,ShirtNumber,Position,ClubId,TeamId,NationalityId")]
            Player model)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            var player = _unitOfWork.players
                .SelectOne(x => x.Id == id);

            if (player == null)
                return NotFound();

            ValidatePlayer(model, id);

            // Existing attendance must remain associated
            // with the correct team's training sessions.
            if (player.TeamId != model.TeamId &&
                _unitOfWork.attendances.Any(x => x.PlayerId == id))
            {
                ModelState.AddModelError(
                    nameof(Player.TeamId),
                    "Cannot change team while attendance records exist.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(
                    model.ClubId,
                    model.TeamId,
                    model.NationalityId);

                return View(model);
            }

            player.Name = model.Name;
            player.ShirtNumber = model.ShirtNumber;
            player.Position = model.Position;
            player.ClubId = model.ClubId;
            player.TeamId = model.TeamId;
            player.NationalityId = model.NationalityId;

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update player. Check related records.");

                LoadDropdowns(
                    model.ClubId,
                    model.TeamId,
                    model.NationalityId);

                return View(model);
            }

            TempData["s"] = "Player updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var player = _unitOfWork.players.FindById(
                id, "Club", "Team", "Nationality");

            if (player == null)
                return NotFound();

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id <= 0)
                return NotFound();

            var player = _unitOfWork.players
                .SelectOne(x => x.Id == id);

            if (player == null)
                return NotFound();

            if (_unitOfWork.attendances.Any(
                x => x.PlayerId == id))
            {
                TempData["errorData"] =
                    "Cannot delete player with attendance records.";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.players.RemoveOne(player);

            try
            {
                _unitOfWork.CommitChanges();
            }
            catch (DbUpdateException)
            {
                TempData["errorData"] =
                    "Unable to delete player because related records exist.";

                return RedirectToAction(nameof(Index));
            }

            TempData["s"] = "Player deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        // GET: Players/TeamsByClub?clubId=1
        [HttpGet]
        public IActionResult TeamsByClub(int clubId)
        {
            if (!_unitOfWork.clubs.Any(x => x.Id == clubId))
                return NotFound();

            var teams = _unitOfWork.teams.GetAll()
                .Where(x => x.ClubId == clubId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToList();

            return Json(teams);
        }

        private void LoadDropdowns(
            int? clubId = null,
            int? teamId = null,
            int? nationalityId = null)
        {
            ViewBag.Clubs = new SelectList(
                _unitOfWork.clubs.GetAll(),
                "Id", "Name", clubId);

            ViewBag.Teams = new SelectList(
                _unitOfWork.teams.GetAll()
                    .Where(x => clubId.HasValue &&
                                x.ClubId == clubId.Value),
                "Id", "Name", teamId);

            ViewBag.Nationalities = new SelectList(
                _unitOfWork.nationalites.GetAll(),
                "Id", "Country", nationalityId);
        }

        private void ValidatePlayer(
            Player model,
            int? excludeId = null)
        {
            model.Name = (model.Name ?? string.Empty).Trim();
            model.Position = (model.Position ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(Player.Name),
                    "Player name is required");
            }

            if (string.IsNullOrWhiteSpace(model.Position))
            {
                ModelState.AddModelError(
                    nameof(Player.Position),
                    "Position is required");
            }

            if (!_unitOfWork.clubs.Any(
                x => x.Id == model.ClubId))
            {
                ModelState.AddModelError(
                    nameof(Player.ClubId),
                    "Please select a valid club");
            }

            if (model.TeamId.HasValue &&
                !_unitOfWork.teams.Any(x =>
                    x.Id == model.TeamId.Value &&
                    x.ClubId == model.ClubId))
            {
                ModelState.AddModelError(
                    nameof(Player.TeamId),
                    "Selected team does not belong to this club");
            }

            if (model.NationalityId.HasValue &&
                !_unitOfWork.nationalites.Any(
                    x => x.Id == model.NationalityId.Value))
            {
                ModelState.AddModelError(
                    nameof(Player.NationalityId),
                    "Please select a valid nationality");
            }

            if (model.ShirtNumber < 1 ||
                model.ShirtNumber > 99)
            {
                ModelState.AddModelError(
                    nameof(Player.ShirtNumber),
                    "Shirt number must be between 1 and 99");
            }

            if (model.TeamId.HasValue &&
                _unitOfWork.players.Any(x =>
                    x.TeamId == model.TeamId.Value &&
                    x.ShirtNumber == model.ShirtNumber &&
                    (!excludeId.HasValue ||
                     x.Id != excludeId.Value)))
            {
                ModelState.AddModelError(
                    nameof(Player.ShirtNumber),
                    "Shirt number already exists in this team");
            }
        }
    }
}
