using System;
using System.Collections.Generic;

namespace WebBG.Models;

public partial class Menu
{
    public int IdMenu { get; set; }

    public string? Title { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }
}
