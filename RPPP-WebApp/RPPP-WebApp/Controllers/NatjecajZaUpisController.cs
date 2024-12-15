using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.Controllers
{
    public class NatjecajZaUpisController : Controller
    {
        private readonly RPPP08Context _context;

        public NatjecajZaUpisController(RPPP08Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            // ViewData for sorting
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";

            // Define query to fetch data
            var query = from natjecaj in _context.NatjecajZaUpis
                            // Join with the IdUpisas navigation property to get related Prijava
                        let brojPrijava = natjecaj.IdUpisas.Count()  // Counting the related Prijava records
                        select new UpisViewModel
                        {
                            IdNatjecanja = natjecaj.IdNatjecanja,
                            BrojMjesta = natjecaj.BrojMjesta,
                            DatumOtvaranja = natjecaj.DatumOtvaranja,
                            BrojPrijava = brojPrijava  // The count of Prijava
                        };

            // Sorting
            switch (sortOrder)
            {
                case "id_desc":
                    query = query.OrderByDescending(x => x.IdNatjecanja);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(x => x.DatumOtvaranja);
                    break;
                case "date":
                    query = query.OrderBy(x => x.DatumOtvaranja);
                    break;
                default:
                    query = query.OrderBy(x => x.IdNatjecanja);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await query.CountAsync();

            var pagedData = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);
        }
        public IActionResult Create()
        {
            // Fetch all statuses from the StatusNatjecaja table
            var statusList = _context.StatusNatjecajas.ToList();

            // Pass the list to the view via ViewData
            ViewData["StatusList"] = statusList;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatumOtvaranja,BrojMjesta,IdStatus")] NatjecajZaUpi natjecajZaUpi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(natjecajZaUpi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageCreated"] = "Stavka je uspješno kreirana.";
                return RedirectToAction(nameof(Index));
            }

            // If model is not valid, return to the Create view with the current status list
            ViewBag.StatusList = _context.StatusNatjecajas.ToList();
            return View(natjecajZaUpi);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var natjecaj = await _context.NatjecajZaUpis
                .Include(n => n.IdStatusNavigation) // Include the related StatusNatjecaja
                .FirstOrDefaultAsync(m => m.IdNatjecanja == id);

            if (natjecaj == null)
            {
                return NotFound();
            }

            return View(natjecaj);
        }


        // POST: NatjecajZaUpis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var natjecaj = await _context.NatjecajZaUpis.FindAsync(id);
            if (natjecaj != null)
            {
                _context.NatjecajZaUpis.Remove(natjecaj);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageDeleted"] = "Odabrana stavka je uspješno obrisana.";
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: NatjecajZaUpis/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var natjecaj = await _context.NatjecajZaUpis
                                        .Include(n => n.IdStatusNavigation) // include related data
                                        .FirstOrDefaultAsync(m => m.IdNatjecanja == id);

            if (natjecaj == null)
            {
                return NotFound();
            }

            // Ensure you load the list of statuses into ViewData
            var statusList = await _context.StatusNatjecajas.ToListAsync();
            ViewData["StatusList"] = new SelectList(statusList, "IdStatus", "StatusNatjecanja");

            return View(natjecaj);
        }


        // POST: NatjecajZaUpis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNatjecanja,DatumOtvaranja,BrojMjesta,IdStatus")] NatjecajZaUpi natjecajZaUpi)
        {
            if (id != natjecajZaUpi.IdNatjecanja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(natjecajZaUpi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessageEdited"] = "Odabrana stavka je uspješno uređena.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessageEdited"] = "Odabrana stavka nije uređena.";
                    if (!NatjecajZaUpiExists(natjecajZaUpi.IdNatjecanja))
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

            // If something goes wrong, re-load the statuses and return the view again with the current model
            ViewData["StatusList"] = new SelectList(await _context.StatusNatjecajas.ToListAsync(), "IdStatus", "StatusNatjecanja");
            return View(natjecajZaUpi);
        }

        private bool NatjecajZaUpiExists(int id)
        {
            return _context.NatjecajZaUpis.Any(e => e.IdNatjecanja == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the NatjecajZaUpis entity with related Prijava entities
            var natjecaj = await _context.NatjecajZaUpis
                .Include(n => n.IdUpisas) // Load related Prijava
                .FirstOrDefaultAsync(n => n.IdNatjecanja == id);

            if (natjecaj == null)
            {
                return NotFound();
            }

            return View(natjecaj);
        }



    }
}
