using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class VrstaOdlukeController : Controller
    {
        private readonly RPPP08Context _context;

        public VrstaOdlukeController(RPPP08Context context)
        {
            _context = context; 
        }
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";

            var vrsteOdluke = from v in _context.VrstaOdlukes select v;

            switch (sortOrder)
            {
                case "id_desc":
                    vrsteOdluke = vrsteOdluke.OrderByDescending(v => v.IdVrstaOdluke);
                    break;
                case "Naziv":
                    vrsteOdluke = vrsteOdluke.OrderBy(v => v.VrstaOdluke1);
                    break;
                case "naziv_desc":
                    vrsteOdluke = vrsteOdluke.OrderByDescending(v => v.VrstaOdluke1);
                    break;
                default:
                    vrsteOdluke = vrsteOdluke.OrderBy(v => v.IdVrstaOdluke);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await vrsteOdluke.CountAsync();

            var pagedData = await vrsteOdluke
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VrstaOdluke vrsteOdluke)
        {
            if (vrsteOdluke == null)
            {
                ModelState.AddModelError(string.Empty, "Vrsta odluke ne može biti prazna.");
                return View(vrsteOdluke);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(vrsteOdluke);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Greška prilikom spremanja podataka. Pokušajte ponovo.");
                }
            }
            return View(vrsteOdluke);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vrsteOdluke = await _context.VrstaOdlukes.FindAsync(id);
            if (vrsteOdluke == null) return NotFound();

            return View(vrsteOdluke);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VrstaOdluke vrsteOdluke)
        {
            if (vrsteOdluke == null)
            {
                ModelState.AddModelError(string.Empty, "Vrsta odluke ne može biti prazna.");
                return View(vrsteOdluke);
            }

            if (id != vrsteOdluke.IdVrstaOdluke)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vrsteOdluke);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VrstaOdlukeExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Greška prilikom ažuriranja podataka. Pokušajte ponovo.");
                }
            }
            return View(vrsteOdluke);
        }



        private bool VrstaOdlukeExists(int id)
        {
            return _context.VrstaOdlukes.Any(e => e.IdVrstaOdluke == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vrstaOdluke = await _context.VrstaOdlukes
                .FirstOrDefaultAsync(m => m.IdVrstaOdluke == id);

            if (vrstaOdluke == null) return NotFound();

            return View(vrstaOdluke);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vrstaOdluke = await _context.VrstaOdlukes.FindAsync(id);
            if (vrstaOdluke != null)
            {
                _context.VrstaOdlukes.Remove(vrstaOdluke);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
