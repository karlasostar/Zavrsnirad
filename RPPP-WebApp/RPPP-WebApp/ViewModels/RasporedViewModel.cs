using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
	internal class RasporedViewModel
	{
		public IEnumerable<Raspored> Rasporedi { get; set; }
		public PagingInfo PagingInfo { get; set; }
	}
}
