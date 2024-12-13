using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Threading.Tasks;
using System.Linq;


namespace RPPP_WebApp.Controllers
{
    public class TematskoPodrucjeController : Controller
    {
        private readonly RPPP08Context _context;

        public TematskoPodrucjeController(RPPP08Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";

            var tematskoPodrucje = from v in _context.TematskoPodrucjes select v;

            switch (sortOrder)
            {
                case "id_desc":
                    tematskoPodrucje = tematskoPodrucje.OrderByDescending(v => v.IdTematskogPodrucja);
                    break;
                case "Naziv":
                    tematskoPodrucje = tematskoPodrucje.OrderBy(v => v.TematskoPodrucje1);
                    break;
                case "naziv_desc":
                    tematskoPodrucje = tematskoPodrucje.OrderByDescending(v => v.TematskoPodrucje1);
                    break;
                default:
                    tematskoPodrucje = tematskoPodrucje.OrderBy(v => v.IdTematskogPodrucja);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await tematskoPodrucje.CountAsync();

            var pagedData = await tematskoPodrucje.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

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
        public async Task<IActionResult> Create(TematskoPodrucje tematskoPodrucje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tematskoPodrucje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tematskoPodrucje);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tematskoPodrucje = await _context.TematskoPodrucjes.FindAsync(id);
            if (tematskoPodrucje == null) return NotFound();

            return View(tematskoPodrucje);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TematskoPodrucje tematskoPodrucje)
        {
            if (id != tematskoPodrucje.IdTematskogPodrucja) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tematskoPodrucje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TematskoPodrucjeExists(id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tematskoPodrucje);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tematskoPodrucje = await _context.TematskoPodrucjes
                .FirstOrDefaultAsync(m => m.IdTematskogPodrucja == id);

            if (tematskoPodrucje == null) return NotFound();

            return View(tematskoPodrucje);
        }

        private bool TematskoPodrucjeExists(int id)
        {
            return _context.TematskoPodrucjes.Any(e => e.IdTematskogPodrucja == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tematskoPodrucje = await _context.TematskoPodrucjes.FindAsync(id);
            if (tematskoPodrucje != null)
            {
                _context.TematskoPodrucjes.Remove(tematskoPodrucje);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
