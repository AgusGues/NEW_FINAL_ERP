namespace NEW_FINAL_ERP.Models
{
    public class ItemsUom
    {
        public int ItemUOMId { get; set; }

        public int ItemId { get; set; }
        public int UnitId { get; set; }

        public decimal ConversionToBase { get; set; }

        public bool IsBase { get; set; }
        public bool IsDefaultSales { get; set; }
        public bool IsDefaultPurchase { get; set; }

        public string? Barcode { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsInternalBarcode { get; set; }

        


    }
}
