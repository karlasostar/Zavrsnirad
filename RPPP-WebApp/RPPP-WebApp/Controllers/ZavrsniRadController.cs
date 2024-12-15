using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;


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

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;
            var query = (IQueryable<ZavrsniRad>)_context.ZavrsniRads
                .Include(zr => zr.IdTematskogPodrucjaNavigation)
                .Include(zr => zr.IdVijecaNavigation)
                .Include(zr => zr.Student)
                .Include(zr => zr.OdlukeFvs);

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

            var zavrsniRads = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();

            
            var zavrsniRadWithDecisions = zavrsniRads
                .Select(zr => new ZavrsniRadSOdlukom
                {
                    IdRad = zr.IdRad,
                    Naslov = zr.Naslov,
                    Metodologija = zr.Metodologija,
                    Ocjena = zr.Ocjena,
                    Tema = zr.Tema,
                    Opis = zr.Opis,
                    Sazetak = zr.Sazetak,
                    DatumObrane = zr.DatumObrane,
                    IdTematskogPodrucja = zr.IdTematskogPodrucja,
                    IdTematskogPodrucjaNavigation = zr.IdTematskogPodrucjaNavigation,
                    IdVijecaNavigation = zr.IdVijecaNavigation,
                    Student = zr.Student,
                    Oib = zr.Oib,
                    IdUpisa = zr.IdUpisa,
                    IdVijeca = zr.IdVijeca,
                    
                    ZadnjaOdluka = zr.OdlukeFvs
                    .OrderByDescending(od => od.DatumOdluke)
                    .FirstOrDefault()?.OpisOdluke 
                }).ToList();

            var model = new ZavrsniRadViewModel2
            {
                ZavrsniRad = zavrsniRadWithDecisions,
                PagingInfo = pagingInfo
            };

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZavrsniRad zavrsniRad)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var student = await _context.Students.FirstOrDefaultAsync(s => s.Oib == zavrsniRad.Oib);

                    if (student == null)
                    {
                        ModelState.AddModelError("", "Student not found.");
                        await PrepareDropDownLists();
                        return View(zavrsniRad);
                    }

                    zavrsniRad.IdUpisa = student.IdUpisa;
                    _context.Add(zavrsniRad);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Završni rad '{zavrsniRad.Naslov}' uspješno dodan.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, $"Greška prilikom spremanja: {ex.Message}");
                    await PrepareDropDownLists();
                    return View(zavrsniRad);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(zavrsniRad);
            }

        }
        private async Task PrepareDropDownLists()
        {
            ViewBag.TematskaPodrucja = new SelectList(
                await _context.TematskoPodrucjes
                    .OrderBy(tp => tp.TematskoPodrucje1)
                    .Select(z => new { z.IdTematskogPodrucja, z.TematskoPodrucje1 })
                    .ToListAsync(),
                "IdTematskogPodrucja", "TematskoPodrucje1"
            );

            ViewBag.Studenti = new SelectList(
                await _context.Students
                    .OrderBy(s => s.Jmbag)
                    .Select(z => new { z.Oib })
                    .ToListAsync(),
                "Oib", "Oib"
            );

            ViewBag.Povjerenstva = new SelectList(
                await _context.Povjerentsvos
                    .OrderBy(p => p.IdVijeca)
                    .Select(z => new { z.IdVijeca })
                    .ToListAsync(),
                "IdVijeca", "IdVijeca"
            );

            ViewBag.VrsteOdlukes = new SelectList(
                await _context.VrstaOdlukes
                .OrderBy(p => p.IdVrstaOdluke)
                .Select(z => new {z.IdVrstaOdluke, z.VrstaOdluke1 })
                .ToListAsync(),
                "IdVrstaOdluke", "VrstaOdluke1");

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var zavrsniRad = await _context.ZavrsniRads.FindAsync(id);
            if (zavrsniRad == null) return NotFound();
            await PrepareDropDownLists();
            return View(zavrsniRad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ZavrsniRad zavrsniRad)
        {
            if (zavrsniRad == null)
            {
                ModelState.AddModelError(string.Empty, "Vrsta odluke ne može biti prazna.");
                return View(zavrsniRad);
            }

            if (id != zavrsniRad.IdRad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.Oib == zavrsniRad.Oib);

                    if (student == null)
                    {
                        ModelState.AddModelError("", "Student not found.");
                        await PrepareDropDownLists();
                        return View(zavrsniRad);
                    }

                    zavrsniRad.IdUpisa = student.IdUpisa;
                    _context.Update(zavrsniRad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZavrsniRadExists(id))
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
            await PrepareDropDownLists();
            return View(zavrsniRad);
        }


        private bool ZavrsniRadExists(int id)
        {
            return _context.ZavrsniRads.Any(e => e.IdRad == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zavrsniRad = await _context.ZavrsniRads.FindAsync(id);
            if (zavrsniRad != null)
            {
                _context.ZavrsniRads.Remove(zavrsniRad);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalji(int id)
        {
            var zavrsniRad = await _context.ZavrsniRads
                .Include(z => z.IdTematskogPodrucjaNavigation)
                .Include(z => z.IdVijecaNavigation)
                .Include(z => z.Student)
                .Include(z => z.OdlukeFvs)
                .ThenInclude(o => o.IdVrstaOdlukeNavigation)
                .FirstOrDefaultAsync(z => z.IdRad == id);

            if (zavrsniRad == null)
            {
                return NotFound($"Završni rad sa ID-om {id} nije pronađen.");
            }
            ViewBag.VrsteOdlukes = new SelectList(
                        await _context.VrstaOdlukes
                        .OrderBy(p => p.IdVrstaOdluke)
                        .Select(z => new { z.IdVrstaOdluke, z.VrstaOdluke1 })
                        .ToListAsync(),
                         "IdVrstaOdluke", "VrstaOdluke");
            return View(zavrsniRad);
        }

   
        public async Task<IActionResult> EditOdluka(int id)
        {
            var odluka = await _context.OdlukeFvs
                .Include(o => o.IdVrstaOdlukeNavigation)
                .Include(o => o.IdRadNavigation) 
                .FirstOrDefaultAsync(o => o.IdOdluke == id);

            if (odluka == null)
            {
                return NotFound($"Odluka s ID-om {id} nije pronađena.");
            }

            await PrepareDropDownLists(); 
            return View(odluka);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditOdluka(int id, OdlukeFv odluka)
        {
            if (id != odluka.IdOdluke)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure IdRad is set correctly before saving
                    if (odluka.IdRad == 0)
                    {
                        ModelState.AddModelError(string.Empty, "IdRad is missing.");
                        return View(odluka);
                    }

                    _context.Update(odluka);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Detalji), new { id = odluka.IdRad });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OdlukaExists(odluka.IdOdluke))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }

            await PrepareDropDownLists();
            return View(odluka);
        }


        private bool OdlukaExists(int id)
        {
            return _context.OdlukeFvs.Any(e => e.IdOdluke == id);
        }

        public IActionResult DeleteOdluka(int id)
        {
            var odluka = _context.OdlukeFvs.FirstOrDefault(o => o.IdOdluke == id);
            if (odluka == null)
            {
                return NotFound();
            }

            _context.OdlukeFvs.Remove(odluka);
            _context.SaveChanges();

            return RedirectToAction("Detalji", new { id = odluka.IdRad});
        }
        [HttpPost]
        public IActionResult DeleteConfirmedOdluka(int id, int idRad)
        {
            var odluka = _context.OdlukeFvs.FirstOrDefault(o => o.IdOdluke == id);
            if (odluka == null)
            {
                return NotFound();
            }

            _context.OdlukeFvs.Remove(odluka);
            _context.SaveChanges();

            // Redirect to the correct ZavrsniRad details page
            return RedirectToAction("Detalji", new { id = idRad });
        }


        [HttpGet]
        public async Task<IActionResult> CreateOdluka(int id)
        {
            var zavrsniRad = await _context.ZavrsniRads.FindAsync(id);

            if (zavrsniRad == null)
            {
                return NotFound($"Završni rad s ID-om {id} nije pronađen.");
            }

            ViewBag.VrsteOdluke = new SelectList(
                await _context.VrstaOdlukes
                    .OrderBy(v => v.VrstaOdluke1)
                    .ToListAsync(),
                "IdVrstaOdluke", "VrstaOdluke1"
            );

            return View(new OdlukeFv { IdRad = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOdluka(OdlukeFv odluka)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var zavrsniRad = await _context.ZavrsniRads.FindAsync(odluka.IdRad);
                    if (zavrsniRad == null)
                    {
                        ModelState.AddModelError("", "Završni rad nije pronađen.");
                        return View(odluka);
                    }

                    _context.Add(odluka);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Detalji), new { id = odluka.IdRad });
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, $"Greška prilikom spremanja: {ex.Message}");
                }
            }

            ViewBag.VrsteOdluke = new SelectList(
                await _context.VrstaOdlukes
                    .OrderBy(v => v.VrstaOdluke1)
                    .ToListAsync(),
                "IdVrstaOdluke", "VrstaOdluke1"
            );

            return View(odluka);
        }
    }
}
