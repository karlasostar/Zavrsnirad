using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class DvoranaController : Controller
    {
        private readonly RPPP08Context ctx;
        public DvoranaController(RPPP08Context ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            var dvorane = ctx.Dvoranas.AsNoTracking().OrderBy(d => d.OznDvorana).ToList();
            return View(dvorane);
        }
    }
}
