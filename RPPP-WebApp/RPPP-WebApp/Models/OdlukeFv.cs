﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models;

public partial class OdlukeFv
{
    public DateOnly DatumOdluke { get; set; }

    public int IdOdluke { get; set; }

    public string OpisOdluke { get; set; }

    public int IdRad { get; set; }

    public int IdVrstaOdluke { get; set; }

    public virtual ZavrsniRad IdRadNavigation { get; set; }

    public virtual VrstaOdluke IdVrstaOdlukeNavigation { get; set; }

    public virtual ICollection<Fakultetsko> IdVijecas { get; set; } = new List<Fakultetsko>();
}