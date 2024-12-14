using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Threading.Tasks;
using System.Linq;


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

            var statusNatjecaja = from v in _context.StatusNatjecajas select v;

            switch (sortOrder)
            {
                case "id_desc":
                    statusNatjecaja = statusNatjecaja.OrderByDescending(v => v.IdStatus);
                    break;
                case "Naziv":
                    statusNatjecaja = statusNatjecaja.OrderBy(v => v.StatusNatjecanja.Trim());

                    break;
                case "naziv_desc":
                    statusNatjecaja = statusNatjecaja.OrderByDescending(v => v.StatusNatjecanja.Trim());
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
            if (ModelState.IsValid)
            {
                _context.Add(statusNatjecaja);
                await _context.SaveChangesAsync();
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusNatjecaja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusNatjecajaExists(id)) return NotFound();
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
            var statusNatjecaja = await _context.StatusNatjecajas.FindAsync(id);
            if (statusNatjecaja != null)
            {
                _context.Remove(statusNatjecaja);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
