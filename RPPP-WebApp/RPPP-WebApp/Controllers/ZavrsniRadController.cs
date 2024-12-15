using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using RPPP_WebApp.ViewModels;


namespace RPPP_WebApp.Controllers
{
    public class ZavrsniRadController : Controller
    {
        private readonly RPPP08Context _context;

        public ZavrsniRadController(RPPP08Context ctx)
        {
            this._context = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var zavrsniRadovi = await _context.ZavrsniRads
                .Include(z => z.IdTematskogPodrucjaNavigation)
                .Include(z => z.IdVijecaNavigation)
                .Include(z => z.Student)
                .Select(z => new ZavrsniRadViewModel
                {
                    ZavrsniRad = z,
                    ZadnjaOdluka = _context.OdlukeFvs
                        .Where(o => o.IdRad == z.IdRad)
                        .OrderByDescending(o => o.DatumOdluke)
                        .Select(o => o.IdVrstaOdlukeNavigation.VrstaOdluke1)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(zavrsniRadovi);
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
