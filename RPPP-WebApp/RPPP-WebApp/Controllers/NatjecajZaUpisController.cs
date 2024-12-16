using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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
            
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CurrentSort"] = sortOrder;

            
            var query = from natjecaj in _context.NatjecajZaUpis
                            
                        let brojPrijava = natjecaj.IdUpisas.Count()  
                        select new UpisViewModel
                        {
                            IdNatjecanja = natjecaj.IdNatjecanja,
                            BrojMjesta = natjecaj.BrojMjesta,
                            DatumOtvaranja = natjecaj.DatumOtvaranja,
                            BrojPrijava = brojPrijava  
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
            
            var statusList = _context.StatusNatjecajas.ToList();

            
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

            
            ViewData["StatusList"] = new SelectList(await _context.StatusNatjecajas.ToListAsync(), "IdStatus", "StatusNatjecanja");
            return View(natjecajZaUpi);
        }

        private bool NatjecajZaUpiExists(int id)
        {
            return _context.NatjecajZaUpis.Any(e => e.IdNatjecanja == id);
        }

        public async Task<IActionResult> Details(int? id, int? pageNumber, string sortOrder)
        {
            if (id == null)
            {
                return NotFound();
            }

            var natjecaj = await _context.NatjecajZaUpis
                .Include(n => n.IdUpisas) 
                    .ThenInclude(p => p.IdPrijaveNavigation) 
                .FirstOrDefaultAsync(n => n.IdNatjecanja == id);

            if (natjecaj == null)
            {
                return NotFound();
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentSort"] = sortOrder;

            return View(natjecaj);
        }


        
        public IActionResult DeletePrijava(int id)
        {
            var prijava = _context.Prijavas
                .FirstOrDefault(p => p.IdPrijave == id);

            if (prijava == null)
            {
                return NotFound();  // If not found, return a 404 error
            }

            return View(prijava); 
        }

        
        [HttpPost, ActionName("DeletePrijava")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePrijavaConfirmed(int id)
        {
            var prijava = _context.Prijavas.FirstOrDefault(p => p.IdPrijave == id);

            if (prijava != null)
            {
                
                _context.Prijavas.Remove(prijava);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");  // Redirect to the list page or appropriate view after deletion
        }





        public IActionResult CreatePrijava(int idNatjecanja)
        {
            
            var statusPrijaveList = _context.StatusPrijaves
                .Select(status => new SelectListItem
                {
                    Value = status.IdPrijave.ToString(), // Use the IdPrijave for the value
                    Text = status.StatusPrijave1 // Use the StatusPrijave1 for the display text
                })
                .ToList();

            
            ViewData["StatusPrijaveList"] = statusPrijaveList;

            
            ViewBag.IdNatjecanja = idNatjecanja;

            return View();
        }






        // POST: Prijava/CreatePrijava
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult CreatePrijava(Prijava prijava, int idNatjecanja, int idPrijave)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the StatusPrijave entity based on the selected IdPrijave
                var statusPrijave = _context.StatusPrijaves
                                            .FirstOrDefault(s => s.IdPrijave == idPrijave);

                if (statusPrijave == null)
                {
                    ModelState.AddModelError("", "Status prijave not found.");
                    return View(prijava);
                }

                // Associate the StatusPrijave with the Prijava
                prijava.IdPrijaveNavigation = statusPrijave;

                // Retrieve the NatjecajZaUpi based on the given idNatjecanja
                var natjecajZaUpi = _context.NatjecajZaUpis
                                            .FirstOrDefault(n => n.IdNatjecanja == idNatjecanja);

                if (natjecajZaUpi == null)
                {
                    ModelState.AddModelError("", "Natjecaj not found.");
                    return View(prijava);
                }

                // Associate the NatjecajZaUpi with the Prijava via the join table (NatjeceSe)
                prijava.IdNatjecanjas.Add(natjecajZaUpi);

                // Add the new Prijava entity to the context
                _context.Prijavas.Add(prijava);

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the details page or list view after successful creation
                return RedirectToAction("Details", new { id = idNatjecanja });
            }

            // If the model state is invalid, reload the form with current data
            var statusPrijaveList = _context.StatusPrijaves.ToList();
            ViewData["StatusPrijaveList"] = new SelectList(statusPrijaveList, "IdPrijave", "StatusPrijave1");
            ViewBag.IdNatjecanja = idNatjecanja;

            return View(prijava);
        }


    }
}
