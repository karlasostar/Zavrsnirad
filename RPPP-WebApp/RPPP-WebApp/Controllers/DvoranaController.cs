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
    public class DvoranaController : Controller
    {
        private readonly RPPP08Context ctx;
        private readonly ILogger<DvoranaController> logger;
        private readonly AppSettings appSettings;
        public DvoranaController(RPPP08Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot, ILogger<DvoranaController> logger)
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

            var query = ctx.Dvoranas
                           .AsNoTracking();

            int count = query.Count();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedna dvorana");
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

            var dvorane = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new DvoranaViewModel
            {
                Dvorane = dvorane,
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
        public IActionResult Create(Dvorana dvorana)
        {
            logger.LogTrace(JsonSerializer.Serialize(dvorana));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(dvorana);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Dvorana {dvorana.OznDvorana} dodana.");

                    TempData["Success"] = $"Dvorana {dvorana.OznDvorana} dodana.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove dvorane: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(dvorana);
                }
            }
            else
            {
                return View(dvorana);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int IdDvorana, int page = 1, int sort = 1, bool ascending = true)
        {
            var dvorana = ctx.Dvoranas.Find(IdDvorana);
            if (dvorana != null)
            {
                try
                {
                    string naziv = dvorana.OznDvorana;
                    ctx.Remove(dvorana);
                    ctx.SaveChanges();
                    logger.LogInformation($"Dvorana {naziv} uspješno obrisana");
                    TempData["Success"] = $"Dvorana {naziv} uspješno obrisana";
                }
                catch (Exception exc)
                {
                    TempData["Error"] = "Nije moguće obrisati ovu stavku zato što se koristi na drugom mjestu.";
                    logger.LogError("Pogreška prilikom brisanja dvorane: " + exc.CompleteExceptionMessage());
                }
            }
            else
            {
                logger.LogWarning("Ne postoji dvorana s oznakom: {0} ", IdDvorana);
                TempData["Error"] = "Ne postoji dvorana s oznakom: " + IdDvorana;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var dvorana = ctx.Dvoranas.AsNoTracking().Where(d => d.IdDvorana == id).SingleOrDefault();
            if (dvorana == null)
            {
                logger.LogWarning("Ne postoji dvorana s id: {0} ", id);
                return NotFound("Ne postoji dvorana s id: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(dvorana);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Dvorana dvorana = await ctx.Dvoranas.FindAsync(id);
                if (dvorana == null)
                {
                    return NotFound("Neispravna oznaka dvorane: " + id);
                }

                if (await TryUpdateModelAsync<Dvorana>(dvorana, "",
                    d => d.OznDvorana, d => d.Kapacitet
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData["Success"] = "Dvorana ažurirana.";
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(dvorana);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
                    return View(dvorana);
                }
            }
            catch (Exception exc)
            {
                TempData["Error"] = exc.CompleteExceptionMessage();
                return RedirectToAction(nameof(Edit), id);
            }
        }
    }
}
