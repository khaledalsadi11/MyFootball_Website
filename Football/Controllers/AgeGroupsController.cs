using Football.Models;
using Football.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Football.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgeGroupsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AgeGroupsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: AgeGroups
        public IActionResult Index()
        {
            var ageGroups = _unitOfWork.ageGroups.GetAll();

            return View(ageGroups);
        }

        // GET: AgeGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AgeGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Name,MinAge,MaxAge")] AgeGroup ageGroup)
        {
            if (_unitOfWork.ageGroups.GetAll()
                .Any(x => x.Name == ageGroup.Name))
            {
                ModelState.AddModelError(
                    nameof(AgeGroup.Name),
                    "Age group already exists");
            }

            if (!ModelState.IsValid)
            {
                return View(ageGroup);
            }

            _unitOfWork.ageGroups.AddOne(ageGroup);
            _unitOfWork.CommitChanges();

            TempData["s"] = "Age group added successfully";

            return RedirectToAction(nameof(Index));
        }


        // GET: AgeGroups/Edit/5
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return NotFound();

          
            var ageGroup = _unitOfWork.ageGroups.SelectOne(x => x.Id == id);

            if (ageGroup == null)
                return NotFound();

            return View(ageGroup);
        }

        // POST: AgeGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Name,MinAge,MaxAge")] AgeGroup model)
        {
            if (id != model.Id)
                return BadRequest();

            var ageGroup = _unitOfWork.ageGroups
                .GetAll()
                .FirstOrDefault(x => x.Id == id);

            if (ageGroup == null)
                return NotFound();

            bool exists = _unitOfWork.ageGroups.Any(x => x.Name == model.Name && x.Id != id);

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(AgeGroup.Name),
                    "Age group name already exists");
            }

            if (!ModelState.IsValid)
                return View(model);

            ageGroup.Name = model.Name;
            ageGroup.MinAge = model.MinAge;
            ageGroup.MaxAge = model.MaxAge;

            _unitOfWork.CommitChanges();

            TempData["s"] = "Age group updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // GET: AgeGroups/Delete/5
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var ageGroup = _unitOfWork.ageGroups
            .SelectOne(x => x.Id == id);


            if (ageGroup == null)
                return NotFound();

            return View(ageGroup);
        }

        // POST: AgeGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
           
       var ageGroup = _unitOfWork.ageGroups.SelectOne(x => x.Id == id);

            if (ageGroup == null)
                return NotFound();

            // Prevent deleting an age group used by teams
            bool hasTeams = _unitOfWork.teams
                .Any(x => x.AgeGroupId == id);

            if (hasTeams)
            {
                TempData["errorData"] =
                    "Cannot delete an age group assigned to teams";

                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.ageGroups.RemoveOne(ageGroup);
            _unitOfWork.CommitChanges();

            TempData["s"] = "Age group deleted successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}