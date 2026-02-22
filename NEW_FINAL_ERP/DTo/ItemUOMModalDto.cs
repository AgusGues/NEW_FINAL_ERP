namespace NEW_FINAL_ERP.DTo
{
    public class ItemUOMModalDto
    {
        
            public int ItemUOMId { get; set; } = 0;   // 0 = Create, >0 = Edit
            public int ItemId { get; set; }
            public int UnitId { get; set; }
            public decimal ConversionToBase { get; set; }
            public bool IsBase { get; set; }
            public bool IsDefaultSales { get; set; }
            public bool IsDefaultPurchase { get; set; }
            public string? Barcode { get; set; }

            public IEnumerable<ItemDto> Items { get; set; } = Enumerable.Empty<ItemDto>();
            public IEnumerable<UnitDto> Units { get; set; } = Enumerable.Empty<UnitDto>();
        }

        public class ItemDto
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public string ItemCode { get; set; } = string.Empty;
        }

        public class UnitDto
        {
            public int UnitId { get; set; }
            public string UnitName { get; set; } = string.Empty;
        }
    }

