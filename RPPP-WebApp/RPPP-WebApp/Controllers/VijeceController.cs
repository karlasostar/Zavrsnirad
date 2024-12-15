using Microsoft.AspNetCore.Mvc;
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
            var vijeca = await context.Vijećes
                .Include(v => v.IdAkGodNavigation) 
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
                    sjednice = v.Sjednicas 
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
