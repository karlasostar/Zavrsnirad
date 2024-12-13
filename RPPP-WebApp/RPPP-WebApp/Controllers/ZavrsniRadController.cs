using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace RPPP_WebApp.Controllers
{
    public class ZavrsniRadController : Controller
    {
        private readonly RPPP08Context _context;
        private readonly ILogger<ZavrsniRadController> logger;
        private readonly AppSettings appData;

        public ZavrsniRadController(RPPP08Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ZavrsniRadController> logger)
        {
            this._context = ctx;
            this.logger = logger;
            appData = options.Value;
        }

        // GET: ZavrsniRad/Index
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pageSize = appData.PageSize;

            var query = _context.ZavrsniRads.AsQueryable();
            int count = await query.CountAsync();

            //query = query.ApplySort(sort, ascending)

            var zavrsniRadovi = _context.ZavrsniRads
                .Include(z => z.IdTematskogPodrucjaNavigation)
                .Include(z => z.IdVijecaNavigation)
                .Include(z => z.Student);

            return View(await zavrsniRadovi.ToListAsync());
        }

        // GET: ZavrsniRad/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IdTematskogPodrucja"] = new SelectList(_context.TematskoPodrucjes, "Id", "Naziv");
            ViewData["IdVijeca"] = new SelectList(_context.Povjerentsvos, "Id", "Naziv");
            ViewData["Oib"] = new SelectList(_context.Students, "Oib", "ImePrezime");
            return View();
        }

        // POST: ZavrsniRad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZavrsniRad zavrsniRad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zavrsniRad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTematskogPodrucja"] = new SelectList(_context.TematskoPodrucjes, "Id", "Naziv", zavrsniRad.IdTematskogPodrucja);
            ViewData["IdVijeca"] = new SelectList(_context.Povjerentsvos, "Id", "Naziv", zavrsniRad.IdVijeca);
            ViewData["Oib"] = new SelectList(_context.Students, "Oib", "ImePrezime", zavrsniRad.Oib);
            return View(zavrsniRad);
        }

        // GET: ZavrsniRad/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zavrsniRad = await _context.ZavrsniRads.FindAsync(id);
            if (zavrsniRad == null)
            {
                return NotFound();
            }
            ViewData["IdTematskogPodrucja"] = new SelectList(_context.TematskoPodrucjes, "Id", "Naziv", zavrsniRad.IdTematskogPodrucja);
            ViewData["IdVijeca"] = new SelectList(_context.Povjerentsvos, "Id", "Naziv", zavrsniRad.IdVijeca);
            ViewData["Oib"] = new SelectList(_context.Students, "Oib", "ImePrezime", zavrsniRad.Oib);
            return View(zavrsniRad);
        }

        // POST: ZavrsniRad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ZavrsniRad zavrsniRad)
        {
            if (id != zavrsniRad.IdRad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zavrsniRad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZavrsniRadExists(zavrsniRad.IdRad))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTematskogPodrucja"] = new SelectList(_context.TematskoPodrucjes, "Id", "Naziv", zavrsniRad.IdTematskogPodrucja);
            ViewData["IdVijeca"] = new SelectList(_context.Povjerentsvos, "Id", "Naziv", zavrsniRad.IdVijeca);
            ViewData["Oib"] = new SelectList(_context.Students, "Oib", "ImePrezime", zavrsniRad.Oib);
            return View(zavrsniRad);
        }

        // GET: ZavrsniRad/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zavrsniRad = await _context.ZavrsniRads
                .Include(z => z.IdTematskogPodrucjaNavigation)
                .Include(z => z.IdVijecaNavigation)
                .Include(z => z.Student)
                .FirstOrDefaultAsync(m => m.IdRad == id);

            if (zavrsniRad == null)
            {
                return NotFound();
            }

            return View(zavrsniRad);
        }

        // POST: ZavrsniRad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zavrsniRad = await _context.ZavrsniRads.FindAsync(id);
            _context.ZavrsniRads.Remove(zavrsniRad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZavrsniRadExists(int id)
        {
            return _context.ZavrsniRads.Any(e => e.IdRad == id);
        }
    }

}



