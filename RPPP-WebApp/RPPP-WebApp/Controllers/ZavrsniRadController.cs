using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using RPPP_WebApp.ViewModel;


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

        // POST: ZavrsniRad/Create
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

        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var zavrsniRad = await _context.ZavrsniRads
        //        .FirstOrDefaultAsync(m => m.IdRad == id);

        //    if (zavrsniRad == null) return NotFound();

        //    return View(zavrsniRad);
        //}

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

            return View(zavrsniRad);
        }



    }
}
