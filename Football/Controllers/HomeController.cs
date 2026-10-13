using Football.Data;
using Football.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Football.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
           IEnumerable<Club> HomeClubs =_context.Clubs.Include(x=>x.League).ToList();

            return View(HomeClubs);
        }

      
    }
}
