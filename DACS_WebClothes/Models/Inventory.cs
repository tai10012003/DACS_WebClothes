using System;
using System.Collections.Generic;

namespace DACS_WebClothes.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? ProductId { get; set; }

    public string? PlaceOfImport { get; set; }

    public DateTime? ImportDate { get; set; }

    public decimal? ImportPrice { get; set; }

    public int? InventoryNumber { get; set; }

    public int? OrderIndex { get; set; }

    public int? Hidden { get; set; }

    public virtual Product? Product { get; set; }
}
