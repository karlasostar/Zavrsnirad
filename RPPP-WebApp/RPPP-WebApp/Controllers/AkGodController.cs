using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class AkGodController : Controller
    {
        private readonly RPPP08Context context;

        public AkGodController(RPPP08Context context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";
            ViewData["CurrentSort"] = sortOrder;

            var akgod = from v in context.Akgods select v;

            switch (sortOrder)
            {
                case "id_desc":
                    akgod = akgod.OrderByDescending(v => v.IdAkGod);
                    break;
                case "Naziv":
                    akgod = akgod.OrderBy(v => v.Razdoblje == null ? "" : v.Razdoblje.ToUpper());

                    break;
                case "naziv_desc":
                    akgod = akgod.OrderByDescending(v => v.Razdoblje == null ? "" : v.Razdoblje.ToUpper());
                    break;
                default:
                    akgod = akgod.OrderBy(v => v.IdAkGod);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await akgod.CountAsync();

            var pagedData = await akgod.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);

            /*
            var akGodList = await context.Akgods.ToListAsync();

            return View(akGodList);
            */
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var akgod = await context.Akgods.FindAsync(id);
            if (akgod == null) return NotFound();

            return View(akgod);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdAkGod,Razdoblje")] Akgod akgod)
        {
            var existingAkgod = await context.Akgods.FirstOrDefaultAsync(a => a.Razdoblje == akgod.Razdoblje);

            if (existingAkgod != null)
            {
                ModelState.AddModelError("Razdoblje", "Ova akademska godina vec postoji!");
                return View(akgod);
            }

            if (id != akgod.IdAkGod)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(akgod);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Greška prilikom spremanja promjena.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Uneseni podaci nisu ispravni. Provjerite formu.";
            }

            return View(akgod);
        }
    }
}
