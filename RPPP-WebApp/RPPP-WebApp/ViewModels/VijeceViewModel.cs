using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class VijeceViewModel
    {
        public IEnumerable<VijeceSaSjednicom> Vijece { get; set; }
    }

    public class VijeceSaSjednicom
    {
        public int IdVijece { get; set; }
        public string TipVijeca { get; set; }
        public virtual ICollection<ZavrsniRad> ZavrsniRadovi { get; set; } = new List<ZavrsniRad>();
        public virtual Vijeće StrucnoVijece { get; set; }
        public virtual ICollection<Vijeće> Povjerenstva { get; set; } = new List<Vijeće>();
        public virtual ICollection<OdlukeFv> OdlukeFvs { get; set; } = new List<OdlukeFv>();
        public virtual Akgod Akgod { get; set; }
        public virtual Akgod IdAkGodNavigation { get; set; }
        public virtual ICollection<VijeceUlogaZap> VijeceUlogaZaps { get; set; } = new List<VijeceUlogaZap>();
        public virtual ICollection<Sjednica> sjednice { get; set; } = new List<Sjednica>();

    }
}