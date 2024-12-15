using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class VrstaOdlukeController : Controller
    {
        private readonly RPPP08Context _context;
        private readonly ILogger<TematskoPodrucjeController> logger;
        private readonly AppSettings appData;

        public VrstaOdlukeController(RPPP08Context context, IOptionsSnapshot<AppSettings> options, ILogger<TematskoPodrucjeController> logger)
        {
            _context = context;
            this.logger = logger;
            appData = options.Value;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;
            var query = _context.VrstaOdlukes.AsNoTracking(); //or AsQueryable()
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

            var odluke = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();

            var model = new VrstaOdlukeViewModel
            {
                VrstaOdluke = odluke,
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
