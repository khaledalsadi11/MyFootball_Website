using Football.Data;
using Football.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Football.Controllers
{
    public class ClubsController : Controller
    {
        private readonly AppDbContext _context;
        public ClubsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Club> clubs = _context.Clubs.ToList();
            return View(clubs);
        }

        //Get 
        public IActionResult Create()
        {
            return View();
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Club club)
        {
            var check = _context.Clubs.Any(x => x.Name == club.Name);
            if (check) 
            {
                ModelState.AddModelError("Name", "Club Name Already Exists");
            }

            if (!ModelState.IsValid)
            {
                return View(club);
            }
                _context.Clubs.Add(club);
                _context.SaveChanges();
                TempData["s"] = "Club Added Succesfully";
                return RedirectToAction("Index");

            }
        


    //Get 
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0) 
            {
                return NotFound();
            }

            var club = _context.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Club club)
        {

            if (!ModelState.IsValid)
            {
                return View(club);
            }
            _context.Clubs.Update(club);
            _context.SaveChanges();
            TempData["s"] = "Club Updated Succesfully";

            return RedirectToAction("Index");

        }




        //Get 
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var club = _context.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Club club)
        {

            if (!ModelState.IsValid)
            {
                return View(club);
            }
            _context.Clubs.Remove(club);
            _context.SaveChanges();
            TempData["s"] = "Club Deleted Succesfully";

            return RedirectToAction("Index");

        }

        //Get 
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var club = _context.Clubs.Include(x=>x.Players).ThenInclude(x=>x.Nationality).SingleOrDefault(c=>c.Id == id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }

    }
}

