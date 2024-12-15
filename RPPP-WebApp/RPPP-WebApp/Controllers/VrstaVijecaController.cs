using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class VrstaVijecaController : Controller
    {
        private readonly RPPP08Context context;

        public VrstaVijecaController(RPPP08Context context)
        {
            this.context = context;
        }
    }
}
