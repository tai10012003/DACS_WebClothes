using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Title { get; set; }

    public int? OrderIndex { get; set; }

    public string? Link { get; set; }

    public int? Hidden { get; set; }
}
