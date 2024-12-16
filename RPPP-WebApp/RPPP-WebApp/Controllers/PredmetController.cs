using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class PredmetController : Controller
    {
        private readonly RPPP08Context ctx;
        private readonly ILogger<PredmetController> logger;
        private readonly AppSettings appSettings;
        public PredmetController(RPPP08Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot, ILogger<PredmetController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = optionsSnapshot.Value;
        }

        [HttpGet]
        public IActionResult Stvori()
        {
            return View();
        }


        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Predmets
                           .AsNoTracking();

            int count = query.Count();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedan predmet");
                return RedirectToAction(nameof(Create));
            }

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

            var predmeti = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new PredmetViewModel
            {
                Predmeti = predmeti,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Predmet predmet)
        {
            var existingPredmet = ctx.Predmets.FirstOrDefault(a => a.Naziv == predmet.Naziv);

            if (existingPredmet != null)
            {
                ModelState.AddModelError("Naziv", "Predmet sa ovim imenom vec postoji!");
                return View(predmet);
            }

            logger.LogTrace(JsonSerializer.Serialize(predmet));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(predmet);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Predmet {predmet.Naziv} dodan.");

                    TempData["Success"] = $"Predmet {predmet.Naziv} dodan.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog predmeta: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(predmet);
                }
            }
            else
            {
                return View(predmet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int SifPredmet, int page = 1, int sort = 1, bool ascending = true)
        {
            var predmet = ctx.Predmets.Find(SifPredmet);
            if (predmet != null)
            {
                try
                {
                    string naziv = predmet.Naziv;
                    ctx.Remove(predmet);
                    ctx.SaveChanges();
                    logger.LogInformation($"Predmet {naziv} uspješno obrisan");
                    TempData["Success"] = $"Predmet {naziv} uspješno obrisan";
                }
                catch (Exception exc)
                {
                    TempData["Error"] = "Nije moguće obrisati ovu stavku zato što se koristi na drugom mjestu.";
                    logger.LogError("Pogreška prilikom brisanja dvorane: " + exc.CompleteExceptionMessage());
                }
            }
            else
            {
                logger.LogWarning("Ne postoji predmet s oznakom: {0} ", SifPredmet);
                TempData["Error"] = "Ne postoji predmet s oznakom: " + SifPredmet;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var predmet = ctx.Predmets.AsNoTracking().Where(d => d.SifPredmet == id).SingleOrDefault();
            if (predmet == null)
            {
                logger.LogWarning("Ne postoji predmet s sifrom: {0} ", id);
                return NotFound("Ne postoji predmet s sifrom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(predmet);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Predmet predmet = await ctx.Predmets.FindAsync(id);
                if (predmet == null)
                {
                    return NotFound("Neispravna oznaka predmeta: " + id);
                }

                string originalNaziv = predmet.Naziv;

                if (await TryUpdateModelAsync<Predmet>(predmet, "",
                    d => d.Naziv, d => d.PlanProgram, d => d.Program, d => d.JelIzboran, d => d.Ects
                ))
                {
                    if (originalNaziv != predmet.Naziv)
                    {
                        bool nazivPredmetaExists = await ctx.Predmets
                            .AnyAsync(p => p.Naziv == predmet.Naziv && p.SifPredmet != id);

                        if (nazivPredmetaExists)
                        {
                            ModelState.AddModelError(nameof(Predmet.Naziv), "Predmet s ovim nazivom već postoji.");
                            return View(predmet);
                        }
                    }

                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData["Success"] = "Predmet ažuriran.";
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(predmet);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o predmetu nije moguće povezati s forme");
                    return View(predmet);
                }
            }
            catch (Exception exc)
            {
                TempData["Error"] = "Nije moguće urediti ovu stavku";
                return RedirectToAction(nameof(Edit), id);
            }
        }
    }
}
