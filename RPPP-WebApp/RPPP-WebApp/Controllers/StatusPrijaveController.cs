using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Threading.Tasks;
using System.Linq;


namespace RPPP_WebApp.Controllers
{
    public class StatusPrijaveController : Controller
    {
        private readonly RPPP08Context _context;

        public StatusPrijaveController(RPPP08Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";

            var statusPrijave = from v in _context.StatusPrijaves select v;

            switch (sortOrder)
            {
                case "id_desc":
                    statusPrijave = statusPrijave.OrderByDescending(v => v.IdPrijave);
                    break;
                case "Naziv":
                    statusPrijave = statusPrijave.OrderBy(v => v.StatusPrijave1 == null ? "" : v.StatusPrijave1.ToUpper());
                    break;
                case "naziv_desc":
                    statusPrijave = statusPrijave.OrderByDescending(v => v.StatusPrijave1 == null ? "" : v.StatusPrijave1.ToUpper());
                    break;
                default:
                    statusPrijave = statusPrijave.OrderBy(v => v.IdPrijave);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await statusPrijave.CountAsync();

            var pagedData = await statusPrijave.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

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
        public async Task<IActionResult> Create(StatusPrijave statusPrijave)
        {

            var existingStatusPrijave = await _context.StatusPrijaves.FirstOrDefaultAsync(a => a.StatusPrijave1 == statusPrijave.StatusPrijave1);

            if (existingStatusPrijave != null)
            {
                ModelState.AddModelError("StatusPrijave1", "Status prijave sa ovim imenom vec postoji!");
                return View(statusPrijave);
            }

            if (ModelState.IsValid)
            {
                _context.Add(statusPrijave);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageCreated"] = "Stavka je uspješno kreirana.";
                return RedirectToAction(nameof(Index));
            }
            return View(statusPrijave);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            

            var statusPrijave = await _context.StatusPrijaves.FindAsync(id);

            if (statusPrijave == null) return NotFound();

            return View(statusPrijave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StatusPrijave statusPrijave)
        {
            if (id != statusPrijave.IdPrijave) return NotFound();


            var existingStatusPrijave = await _context.StatusPrijaves.FirstOrDefaultAsync(a => a.StatusPrijave1 == statusPrijave.StatusPrijave1);

            if (existingStatusPrijave != null)
            {
                ModelState.AddModelError("StatusPrijave1", "Status prijave sa ovim imenom vec postoji!");
                return View(statusPrijave);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusPrijave);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessageEdited"] = "Odabrana stavka je uspješno uređena.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusPrijaveExists(id)) return NotFound();
                    TempData["ErrorMessageEdited"] = "Odabrana stavka nije uređena.";
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(statusPrijave);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var statusPrijave = await _context.StatusPrijaves
                .FirstOrDefaultAsync(m => m.IdPrijave == id);

            if (statusPrijave == null) return NotFound();

            return View(statusPrijave);
        }

        private bool StatusPrijaveExists(int id)
        {
            return _context.StatusPrijaves.Any(e => e.IdPrijave == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var associatedRecords = await _context.NatjecajZaUpis
                    .Where(n => n.IdStatus == id)
                    .FirstOrDefaultAsync();

            if (associatedRecords != null)
            {
                TempData["ErrorMessageDeleted"] = "Odabrana stavka se ne može obrisati, dozvoljeno je uređivanje.";
                return RedirectToAction(nameof(Index));
            }

            var statusPrijave = await _context.StatusPrijaves.FindAsync(id);
            if (statusPrijave != null)
            {
                _context.StatusPrijaves.Remove(statusPrijave);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageDeleted"] = "Stavka je uspješno obrisana.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
