using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RPPP_WebApp.Controllers
{
    public class AkGodController : Controller
    {
        private readonly RPPP08Context context;

        public AkGodController(RPPP08Context context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var akGodList = await context.Akgods.ToListAsync();

            return View(akGodList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Create(Akgod akgod)
        {
            var existingAkgod = await context.Akgods.FirstOrDefaultAsync(a => a.Razdoblje == akgod.Razdoblje);

            if (existingAkgod != null)
            {
                ModelState.AddModelError("Razdoblje", "Ova akademska godina vec postoji!");
                return View(akgod);
            }

            if (ModelState.IsValid)
            {
                context.Add(akgod);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(akgod);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var akgod = await context.Akgods.Include(a => a.Vijećes) .FirstOrDefaultAsync(a => a.IdAkGod == id);

            if (akgod.Vijećes.Any())
            {
                TempData["ErrorMessage"] = "Ne mozete izbrisati akademsku godinu jer postoji vijece koje ju koristi.";
                return RedirectToAction(nameof(Index));
            }

            if (akgod != null)
            {
                context.Akgods.Remove(akgod);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
