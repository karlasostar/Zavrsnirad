using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RPPP_WebApp.Controllers
{
    public class VrstaSjedniceController : Controller
    {
        private readonly RPPP08Context context;

        public VrstaSjedniceController(RPPP08Context context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";
            ViewData["CurrentSort"] = sortOrder;

            var vrsta = from v in context.VrstaSjednices select v;

            switch (sortOrder)
            {
                case "id_desc":
                    vrsta = vrsta.OrderByDescending(v => v.IdVrsteSjednice);
                    break;
                case "Naziv":
                    vrsta = vrsta.OrderBy(v => v.NazivVrsteSjednice == null ? "" : v.NazivVrsteSjednice.ToUpper());
                    break;
                case "naziv_desc":
                    vrsta = vrsta.OrderByDescending(v => v.NazivVrsteSjednice == null ? "" : v.NazivVrsteSjednice.ToUpper());
                    break;
                default:
                    vrsta = vrsta.OrderBy(v => v.IdVrsteSjednice);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await vrsta.CountAsync();

            var pagedData = await vrsta.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VrstaSjednice vrsta)
        {
            var existingVrstaSjednice = await context.VrstaSjednices.FirstOrDefaultAsync(a => a.NazivVrsteSjednice == vrsta.NazivVrsteSjednice);

            if (existingVrstaSjednice != null)
            {
                ModelState.AddModelError("NazivVrsteSjednice", "Vrsta sjednice sa ovim imenom vec postoji!");
                return View(vrsta);
            }

            if (ModelState.IsValid)
            {
                context.Add(vrsta);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(vrsta);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vrsta = await context.VrstaSjednices.Include(a => a.Sjednicas).FirstOrDefaultAsync(a => a.IdVrsteSjednice == id);

            if (vrsta.Sjednicas.Any())
            {
                TempData["ErrorMessage"] = "Ne mozete izbrisati vrstu sjednice jer postoji sjednica koja ju koristi.";
                return RedirectToAction(nameof(Index));
            }

            if (vrsta != null)
            {
                context.VrstaSjednices.Remove(vrsta);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vrsta = await context.VrstaSjednices.FindAsync(id);
            if (vrsta == null) return NotFound();

            return View(vrsta);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdVrsteSjednice,NazivVrsteSjednice")] VrstaSjednice vrsta)
        {
            var existingVrsta = await context.VrstaSjednices.FirstOrDefaultAsync(a => a.NazivVrsteSjednice == vrsta.NazivVrsteSjednice);

            if (existingVrsta != null)
            {
                ModelState.AddModelError("NazivVrsteSjednice", "Ova vrsta sjednice vec postoji!");
                return View(vrsta);
            }

            if (id != vrsta.IdVrsteSjednice)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(vrsta);
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

            return View(vrsta);
        }
    }
}
