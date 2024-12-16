using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;


namespace RPPP_WebApp.Controllers
{
    public class StatusNatjecajaController : Controller
    {
        private readonly RPPP08Context _context;

        public StatusNatjecajaController(RPPP08Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";
            ViewData["CurrentSort"] = sortOrder;

            var statusNatjecaja = from v in _context.StatusNatjecajas select v;

            switch (sortOrder)
            {
                case "id_desc":
                    statusNatjecaja = statusNatjecaja.OrderByDescending(v => v.IdStatus);
                    break;
                case "Naziv":
                    statusNatjecaja = statusNatjecaja.OrderBy(v => v.StatusNatjecanja == null ? "": v.StatusNatjecanja.ToUpper());

                    break;
                case "naziv_desc":
                    statusNatjecaja = statusNatjecaja.OrderByDescending(v => v.StatusNatjecanja == null ? "" : v.StatusNatjecanja.ToUpper());
                    break;
                default:
                    statusNatjecaja = statusNatjecaja.OrderBy(v => v.IdStatus);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await statusNatjecaja.CountAsync();

            var pagedData = await statusNatjecaja.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

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
        public async Task<IActionResult> Create(StatusNatjecaja statusNatjecaja)
        {

            var existingStatusNatjecaja = await _context.StatusNatjecajas.FirstOrDefaultAsync(a => a.StatusNatjecanja == statusNatjecaja.StatusNatjecanja);

            if (existingStatusNatjecaja != null)
            {
                ModelState.AddModelError("StatusNatjecanja", "Status natječaja sa ovim imenom vec postoji!");
                return View(statusNatjecaja);
            }

            if (ModelState.IsValid)
            {
                _context.Add(statusNatjecaja);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageCreated"] = "Stavka je uspješno kreirana.";
                return RedirectToAction(nameof(Index));
                
            }
            return View(statusNatjecaja);
        }

        public async Task<IActionResult> Edit(int? id)

        {


            if (id == null) return NotFound();

            var statusNatjecaja = await _context.StatusNatjecajas.FindAsync(id);
            if (statusNatjecaja == null) return NotFound();

            return View(statusNatjecaja);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StatusNatjecaja statusNatjecaja)
        {
            if (id != statusNatjecaja.IdStatus) return NotFound();

            var existingStatusNatjecaja = await _context.StatusNatjecajas.FirstOrDefaultAsync(a => a.StatusNatjecanja == statusNatjecaja.StatusNatjecanja);

            if (existingStatusNatjecaja != null)
            {
                ModelState.AddModelError("StatusNatjecanja", "Status natječaja sa ovim imenom vec postoji!");
                return View(statusNatjecaja);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusNatjecaja);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessageEdited"] = "Odabrana stavka je uspješno uređena.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusNatjecajaExists(id)) return NotFound();
                    TempData["ErrorMessageEdited"] = "Odabrana stavka nije uređena.";
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(statusNatjecaja);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var statusNatjecaja = await _context.StatusNatjecajas
                .FirstOrDefaultAsync(m => m.IdStatus == id);

            if (statusNatjecaja == null) return NotFound();

            return View(statusNatjecaja);
        }

        private bool StatusNatjecajaExists(int id)
        {
            return _context.StatusNatjecajas.Any(e => e.IdStatus == id);
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

            var statusNatjecaja = await _context.StatusNatjecajas.FindAsync(id);
            if (statusNatjecaja != null)
            {
                _context.Remove(statusNatjecaja);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageDeleted"] = "Odabrana stavka je uspješno obrisana.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
