using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class VrstaOdlukeViewModel
    {
        public IEnumerable<VrstaOdluke> VrstaOdluke { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
