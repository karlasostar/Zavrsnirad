﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models;

public partial class Sjednica
{
    public int IdSjednica { get; set; }

    public int RBr { get; set; }

    public DateOnly DatumSjednice { get; set; }

    public int IdVijeca { get; set; }

    public int IdVrsteSjednice { get; set; }

    public int IdDvorana { get; set; }

    public virtual Dvorana IdDvoranaNavigation { get; set; }

    public virtual Vijeće IdVijecaNavigation { get; set; }

    public virtual VrstaSjednice IdVrsteSjedniceNavigation { get; set; }
}