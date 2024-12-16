using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    internal class DvoranaViewModel
    {
        public IEnumerable<Dvorana> Dvorane { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}