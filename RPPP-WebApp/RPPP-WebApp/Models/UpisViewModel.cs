using System;

namespace RPPP_WebApp.Models
{
    public class UpisViewModel
    {
        public int IdNatjecanja { get; set; }
        public DateOnly DatumOtvaranja { get; set; }
        public int BrojMjesta { get; set; } //ostavljamo ali detalajan kolko ih je prijavljeno je posebno

        public int BrojPrijava { get; set; }
        public int BrUpisa { get; set; }
        public int IdPrijave { get; set; } //nije bitno detalji
        public string StatusUpisa { get; set; } //prikazuj bitno je 
    }
}