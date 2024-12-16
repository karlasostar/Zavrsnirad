using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class VijeceController : Controller
    {
        private readonly RPPP08Context context;

        public VijeceController(RPPP08Context context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {

            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";
            ViewData["CurrentSort"] = sortOrder;

            var vijece = await context.Vijećes
                .Include(v => v.IdAkGodNavigation)
                .Include(v => v.Sjednicas)
                .Include(v => v.Fakultetsko)
                .Include(v => v.Strucno)
                .Include(v => v.Povjerentsvo)
                .Select(v => new VijeceSaSjednicom
                {
                    IdVijece = v.IdVijeca,
                    Akgod = v.IdAkGodNavigation,
                    TipVijeca =
                        v.Fakultetsko != null ? "Fakultetsko" :
                        v.Strucno != null ? "Stručno" :
                        v.Povjerentsvo != null ? "Povjerenstvo" : "Nepoznato",
                    sjednice = v.Sjednicas,
                    Rbr = v.Sjednicas.OrderByDescending(s => s.RBr)
                    .FirstOrDefault().RBr
                })
                .ToListAsync();

            switch (sortOrder)
            {
                case "id_desc":
                    vijece = vijece.OrderByDescending(v => v.IdVijece).ToList();
                    break;
                default:
                    vijece = vijece.OrderBy(v => v.IdVijece).ToList();
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = vijece.Count;

            var pagedData = vijece.Skip((currentPage - 1) * pageSize).Take(pageSize);

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var model = new VijeceViewModel
            {
                Vijece = pagedData
            };

            return View(model);

        }

    }
}
