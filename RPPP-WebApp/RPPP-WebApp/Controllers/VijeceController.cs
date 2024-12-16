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

        public async Task<IActionResult> Index()
        {
            /*
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NazivSortParam"] = sortOrder == "Naziv" ? "naziv_desc" : "Naziv";
            ViewData["CurrentSort"] = sortOrder;

            var vijece = from v in context.Vijećes select v;

            switch (sortOrder)
            {
                case "id_desc":
                    vijece = vijece.OrderByDescending(v => v.IdVijeca);
                    break;
                case "Naziv":
                    vijece = vijece.OrderBy(v => v.Razdoblje == null ? "" : v.Razdoblje.ToUpper());

                    break;
                case "naziv_desc":
                    vijece = vijece.OrderByDescending(v => v.Razdoblje == null ? "" : v.Razdoblje.ToUpper());
                    break;
                default:
                    akgod = akgod.OrderBy(v => v.Id);
                    break;
            }

            int pageSize = 5;
            int currentPage = pageNumber ?? 1;
            int totalRecords = await akgod.CountAsync();

            var pagedData = await akgod.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);
            */

            var vijeca = await context.Vijećes
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
            /*
            var zavrsniRadWithDecisions = vijeca
                .Select(v => new VijeceSaSjednicom
                {
                    
                }).ToList();
            */

            var model = new VijeceViewModel
            {
                Vijece = vijeca
            };

            return View(model);
        }

    }
}
