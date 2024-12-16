using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    internal class PredavanjeViewModel
    {
        public IEnumerable<Predavanje> Predavanja { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public int IdRaspored { get; set; }

        public Raspored Raspored { get; set; }
    }
}
