using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class ZavrsniRadViewModel2

    {
        public IEnumerable<ZavrsniRadSOdlukom> ZavrsniRad { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }

    public class ZavrsniRadSOdlukom
    {
        public int IdRad { get; set; }
        public string Naslov { get; set; }
        public string Metodologija { get; set; }
        public int? Ocjena { get; set; }
        public string Tema { get; set; }
        public string Opis { get; set; }
        public string Sazetak { get; set; }
        public DateOnly? DatumObrane { get; set; }
        public int IdTematskogPodrucja { get; set; }
        public string Oib { get; set; }
        public int IdUpisa { get; set; }
        public int IdVijeca { get; set; }
        public virtual TematskoPodrucje IdTematskogPodrucjaNavigation { get; set; }

        public virtual Povjerentsvo IdVijecaNavigation { get; set; }

        public virtual ICollection<OdlukeFv> OdlukeFvs { get; set; } = new List<OdlukeFv>();

        public virtual Student Student { get; set; }
        public string ZadnjaOdluka { get; set; }
    }
}
