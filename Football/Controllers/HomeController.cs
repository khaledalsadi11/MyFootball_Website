using Football.Data;
using Football.Models;
using Microsoft.AspNetCore.Mvc;
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
           IEnumerable<Club> HomeClubs =_context.Clubs.ToList();

            return View(HomeClubs);
        }

      
    }
}
