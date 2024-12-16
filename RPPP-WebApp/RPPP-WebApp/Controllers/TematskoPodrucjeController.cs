using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;

namespace RPPP_WebApp.Controllers
{
    public class TematskoPodrucjeController : Controller
    {
        private readonly RPPP08Context _context;
        private readonly ILogger<TematskoPodrucjeController> logger;
        private readonly AppSettings appData;

        public TematskoPodrucjeController(RPPP08Context context, IOptionsSnapshot<AppSettings> options, ILogger<TematskoPodrucjeController> logger)
        {
            _context = context;
            this.logger = logger;
            appData = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;
            var query = _context.TematskoPodrucjes.AsNoTracking(); //or AsQueryable()
            int count = await query.CountAsync();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            var podrucja = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();

            var model = new TematskoPodrucjeViewModel
            {
                TematskoPodrucje = podrucja,
                PagingInfo = pagingInfo,
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TematskoPodrucje tematskoPodrucje)
        {
            if( tematskoPodrucje == null)
            {
                ModelState.AddModelError(string.Empty, "Tematsko područje ne može biti prazno");
                return View(tematskoPodrucje);
            }

            if (_context.TematskoPodrucjes.Any(tp => tp.TematskoPodrucje1 == tematskoPodrucje.TematskoPodrucje1))
            {
                ModelState.AddModelError("TematskoPodrucje1", "Tematsko područje s tim nazivom već postoji.");
                return View(tematskoPodrucje);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tematskoPodrucje);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Podrucje {tematskoPodrucje.TematskoPodrucje1} dodano.";
                    return RedirectToAction(nameof(Index));
                }
                catch(DbUpdateException) 
                {
                    ModelState.AddModelError(string.Empty, "Greška prilikom spremanja.");
                }

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
            if(tematskoPodrucje == null)
            {
                ModelState.AddModelError(string.Empty, "Tematsko područje ne može biti prazno.");
                return View(tematskoPodrucje);
            }

            if (_context.TematskoPodrucjes.Any(tp => tp.TematskoPodrucje1 == tematskoPodrucje.TematskoPodrucje1 && tp.IdTematskogPodrucja != id))
            {
                ModelState.AddModelError("TematskoPodrucje1", "Tematsko područje s tim nazivom već postoji.");
                return View(tematskoPodrucje);
            }


            if (id != tematskoPodrucje.IdTematskogPodrucja) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tematskoPodrucje);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TematskoPodrucjeExists(id)) return NotFound();
                    throw;
                }
                catch (DbUpdateException) 
                {
                    ModelState.AddModelError(string.Empty, "Greška prilikom ažuriranja podataka.");
                }
                
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
            if (tematskoPodrucje != null) { 
                try
                {
                    _context.TematskoPodrucjes.Remove(tematskoPodrucje);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Uspješno ste obrisali tematsko područje.";
                } 
                catch(Exception ex)
                {
                    TempData["Error"] = "Nije moguće obrisati ovu stavku jer je to tematsko područje postojećeg završnog rada";
                }
            
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
