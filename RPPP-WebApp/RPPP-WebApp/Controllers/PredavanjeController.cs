using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
	public class PredavanjeController : Controller
	{
		private readonly RPPP08Context ctx;
		private readonly ILogger<PredavanjeController> logger;
		private readonly AppSettings appSettings;
		public PredavanjeController(RPPP08Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot, ILogger<PredavanjeController> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
			appSettings = optionsSnapshot.Value;
		}


		public IActionResult Index(int idRaspored, int page = 1, int sort = 1, bool ascending = true)
		{
			int pagesize = appSettings.PageSize;

			var query = ctx.Predavanjes
						   .AsNoTracking().Where(p => p.IdRaspored == idRaspored);

			int count = query.Count();

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

			var predavanja = query
						.Skip((page - 1) * pagesize)
						.Take(pagesize)
						.Include(r => r.IdRasporedNavigation)
						.Include(r => r.SifPredmetNavigation)
						.ToList();

            var raspored = ctx.Rasporeds
                      .AsNoTracking()
                      .Include(r => r.IdAkGodNavigation)
                      .Include(r => r.IdDvoranaNavigation)
                      .FirstOrDefault(r => r.IdRaspored == idRaspored);

            if (raspored == null)
            {
                TempData["Info"] = "Raspored nije pronađen.";
                return RedirectToAction("Index", "Raspored");
            }

            var model = new PredavanjeViewModel
			{
				Predavanja = predavanja,
				PagingInfo = pagingInfo,
				IdRaspored = idRaspored,
				Raspored = raspored
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Create(int idRaspored)
		{
			var predavanje = new Predavanje
			{
				IdRaspored = idRaspored
			};
			await PrepareDropDownLists();
			return View(predavanje);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Predavanje predavanje)
		{
			logger.LogTrace(JsonSerializer.Serialize(predavanje));
			if (ModelState.IsValid)
			{
				try
				{
					ctx.Add(predavanje);
					await ctx.SaveChangesAsync();
					logger.LogInformation(new EventId(1000), $"Predavanje {predavanje.IdPredavanja} dodan.");

					TempData["Success"] = $"Predavanje {predavanje.IdPredavanja} dodan.";
					return RedirectToAction(nameof(Index), new { predavanje.IdRaspored});
				}
				catch (Exception exc)
				{
					logger.LogError("Pogreška prilikom dodavanje novog predavanjea: {0}", exc.CompleteExceptionMessage());
					ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
					return View(predavanje);
				}
			}
			else
			{
				await PrepareDropDownLists();
				return View(predavanje);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int IdPredavanje, int page = 1, int sort = 1, bool ascending = true)
		{
			var predavanje = ctx.Predavanjes.Find(IdPredavanje);
			if (predavanje != null)
			{
				try
				{
					int naziv = predavanje.IdPredavanja;
					ctx.Remove(predavanje);
					ctx.SaveChanges();
					logger.LogInformation($"Predavanje {naziv} uspješno obrisano");
					TempData["Success"] = $"Predavanje {naziv} uspješno obrisano";
				}
				catch (Exception exc)
				{
					TempData["Error"] = "Pogreška prilikom brisanja predavanja: " + exc.CompleteExceptionMessage();
					logger.LogError("Pogreška prilikom brisanja predavanja: " + exc.CompleteExceptionMessage());
				}
			}
			else
			{
				logger.LogWarning("Ne postoji predavanje s oznakom: {0} ", IdPredavanje);
				TempData["Error"] = "Ne postoji predavanje s oznakom: " + IdPredavanje;
			}
			return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int idRaspored, int id, int page = 1, int sort = 1, bool ascending = true)
		{
			var predavanje = ctx.Predavanjes.AsNoTracking().Where(d => d.IdPredavanja == id).SingleOrDefault();
			if (predavanje == null)
			{
				logger.LogWarning("Ne postoji predavanje s id: {0} ", id);
				return NotFound("Ne postoji predavanje s id: " + id);
			}
			else
			{
				ViewBag.Page = page;
				ViewBag.Sort = sort;
				ViewBag.Ascending = ascending;
				ViewBag.idRaspored = idRaspored;
				await PrepareDropDownLists();
				return View(predavanje);
			}
		}

		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int idRaspored, int id, int page = 1, int sort = 1, bool ascending = true)
		{
			try
			{
				Predavanje predavanje = await ctx.Predavanjes.FindAsync(id);
				if (predavanje == null)
				{
					return NotFound("Neispravna oznaka predavanjea: " + id);
				}

				if (await TryUpdateModelAsync<Predavanje>(predavanje, "",
					d => d.VrijemePocetka, d => d.VrijemeZavrsetka, d => d.IdRaspored, d => d.SifPredmet
				))
				{
					ViewBag.Page = page;
					ViewBag.Sort = sort;
					ViewBag.Ascending = ascending;
					try
					{
						await ctx.SaveChangesAsync();
						TempData["Success"] = "Predavanje ažurirano.";
						await PrepareDropDownLists();
						return RedirectToAction(nameof(Index), new { idRaspored = idRaspored, page = page, sort = sort, ascending = ascending });
					}
					catch (Exception exc)
					{
						ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
						return View(predavanje);
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Podatke o predavanju nije moguće povezati s forme");
					return View(predavanje);
				}
			}
			catch (Exception exc)
			{
				TempData["Error"] = exc.CompleteExceptionMessage();
				return RedirectToAction(nameof(Edit), id);
			}
		}

		private async Task PrepareDropDownLists()
		{
			var rasporedi = await ctx.Rasporeds
								  .OrderBy(d => d.Opis)
								  .ToListAsync();
			var raspored = await ctx.Rasporeds
										.OrderBy(d => d.Opis)
										.FirstOrDefaultAsync();
			var predmeti = await ctx.Predmets
								  .OrderBy(d => d.Naziv)
								  .ToListAsync();
			var predmet = await ctx.Predmets
								  .OrderBy(d => d.Naziv)
								  .FirstOrDefaultAsync();
			ViewBag.Rasporedi = new SelectList(rasporedi, nameof(raspored.IdRaspored), nameof(raspored.Opis));
			ViewBag.Predmeti = new SelectList(predmeti, nameof(predmet.SifPredmet), nameof(predmet.Naziv));
		}
	}
}
