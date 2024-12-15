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
	public class RasporedController : Controller
	{
		private readonly RPPP08Context ctx;
		private readonly ILogger<RasporedController> logger;
		private readonly AppSettings appSettings;
		public RasporedController(RPPP08Context ctx, IOptionsSnapshot<AppSettings> optionsSnapshot, ILogger<RasporedController> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
			appSettings = optionsSnapshot.Value;
		}

		public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
		{
			int pagesize = appSettings.PageSize;

			var query = ctx.Rasporeds
						   .AsNoTracking();

			int count = query.Count();
			if (count == 0)
			{
				logger.LogInformation("Ne postoji nijedan raspored");
				TempData[Constants.Message] = "Ne postoji niti jedan raspored.";
				TempData[Constants.ErrorOccurred] = false;
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

			var rasporedi = query
						.Skip((page - 1) * pagesize)
						.Take(pagesize)
						.Include(r => r.IdAkGodNavigation)
						.Include(r => r.IdDvoranaNavigation)
						.ToList();

			var model = new RasporedViewModel
			{
				Rasporedi = rasporedi,
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
		public async Task<IActionResult> Create(Raspored raspored)
		{
			logger.LogTrace(JsonSerializer.Serialize(raspored));
			if (ModelState.IsValid)
			{
				try
				{
					ctx.Add(raspored);
					await ctx.SaveChangesAsync();
					logger.LogInformation(new EventId(1000), $"Raspored {raspored.IdRaspored} dodan.");

					TempData[Constants.Message] = $"Raspored {raspored.IdRaspored} dodan.";
					TempData[Constants.ErrorOccurred] = false;
					return RedirectToAction(nameof(Index));
				}
				catch (Exception exc)
				{
					logger.LogError("Pogreška prilikom dodavanje novog rasporeda: {0}", exc.CompleteExceptionMessage());
					ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
					return View(raspored);
				}
			}
			else
			{
				await PrepareDropDownLists();
				return View(raspored);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int IdRaspored, int page = 1, int sort = 1, bool ascending = true)
		{
			var raspored = ctx.Rasporeds.Find(IdRaspored);
			if (raspored != null)
			{
				try
				{
					int naziv = raspored.IdRaspored;
					ctx.Remove(raspored);
					ctx.SaveChanges();
					logger.LogInformation($"Raspored {naziv} uspješno obrisan");
					TempData[Constants.Message] = $"Raspored {naziv} uspješno obrisan";
					TempData[Constants.ErrorOccurred] = false;
				}
				catch (Exception exc)
				{
					TempData[Constants.Message] = "Pogreška prilikom brisanja rasporeda: " + exc.CompleteExceptionMessage();
					TempData[Constants.ErrorOccurred] = true;
					logger.LogError("Pogreška prilikom brisanja rasporeda: " + exc.CompleteExceptionMessage());
				}
			}
			else
			{
				logger.LogWarning("Ne postoji raspored s oznakom: {0} ", IdRaspored);
				TempData[Constants.Message] = "Ne postoji raspored s oznakom: " + IdRaspored;
				TempData[Constants.ErrorOccurred] = true;
			}
			return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
		{
			var raspored = ctx.Rasporeds.AsNoTracking().Where(d => d.IdRaspored == id).SingleOrDefault();
			if (raspored == null)
			{
				logger.LogWarning("Ne postoji raspored s id: {0} ", id);
				return NotFound("Ne postoji raspored s id: " + id);
			}
			else
			{
				ViewBag.Page = page;
				ViewBag.Sort = sort;
				ViewBag.Ascending = ascending;
				await PrepareDropDownLists();
				return View(raspored);
			}
		}

		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
		{
			try
			{
				Raspored raspored = await ctx.Rasporeds.FindAsync(id);
				if (raspored == null)
				{
					return NotFound("Neispravna oznaka rasporeda: " + id);
				}

				if (await TryUpdateModelAsync<Raspored>(raspored, "",
					d => d.Opis, d => d.IdAkGod, d => d.IdDvorana
				))
				{
					ViewBag.Page = page;
					ViewBag.Sort = sort;
					ViewBag.Ascending = ascending;
					try
					{
						await ctx.SaveChangesAsync();
						TempData[Constants.Message] = "Raspored ažuriran.";
						TempData[Constants.ErrorOccurred] = false;
						return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
					}
					catch (Exception exc)
					{
						ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
						await PrepareDropDownLists();
						return View(raspored);
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
					return View(raspored);
				}
			}
			catch (Exception exc)
			{
				TempData[Constants.Message] = exc.CompleteExceptionMessage();
				TempData[Constants.ErrorOccurred] = true;
				return RedirectToAction(nameof(Edit), id);
			}
		}

		private async Task PrepareDropDownLists()
		{
			var akGodine = await ctx.Akgods
								  .OrderBy(d => d.Razdoblje)
								  .Select(z => new { z.IdAkGod, z.Razdoblje })
								  .ToListAsync();
			var akGodina = await ctx.Akgods
										.OrderBy(d => d.Razdoblje)
										.FirstOrDefaultAsync();
			var dvorane = await ctx.Dvoranas
								  .OrderBy(d => d.OznDvorana)
								  .ToListAsync();
			var dvorana = await ctx.Dvoranas
								  .OrderBy(d => d.OznDvorana)
								  .FirstOrDefaultAsync();
			ViewBag.AkGodine = new SelectList(akGodine, nameof(akGodina.IdAkGod), nameof(akGodina.Razdoblje));
			ViewBag.Dvorane = new SelectList(dvorane, nameof(dvorana.IdDvorana), nameof(dvorana.OznDvorana));
		}
	}
}
