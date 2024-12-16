using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public class VijeceDTO
    {
        public string TipVijeca { get; set; }
        public int IdVijeca { get; set; }

        public int IdAkGod { get; set; }

        public virtual Fakultetsko Fakultetsko { get; set; }

        public virtual Akgod IdAkGodNavigation { get; set; }

        public virtual Povjerentsvo Povjerentsvo { get; set; }

        public virtual ICollection<Sjednica> Sjednicas { get; set; } = new List<Sjednica>();

        public virtual Strucno Strucno { get; set; }

        public virtual ICollection<VijeceUlogaZap> VijeceUlogaZaps { get; set; } = new List<VijeceUlogaZap>();
    }
}