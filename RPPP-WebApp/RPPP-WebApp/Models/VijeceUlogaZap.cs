﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models;

public partial class VijeceUlogaZap
{
    public int IdVijeceulogazap { get; set; }

    public int IdVijeca { get; set; }

    public int IdUlogaVijece { get; set; }

    public string Oib { get; set; }

    public virtual UlogaVijece IdUlogaVijeceNavigation { get; set; }

    public virtual Vijeće IdVijecaNavigation { get; set; }

    public virtual Zaposlenik OibNavigation { get; set; }
}