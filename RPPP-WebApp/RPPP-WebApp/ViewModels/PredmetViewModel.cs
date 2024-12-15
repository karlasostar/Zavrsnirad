using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    internal class PredmetViewModel
    {
        public IEnumerable<Predmet> Predmeti { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}