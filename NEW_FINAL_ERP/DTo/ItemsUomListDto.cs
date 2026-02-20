namespace NEW_FINAL_ERP.DTo
{
    public class ItemsUomListDto
    {
        public int ItemUOMId { get; set; }
        public int ItemId { get; set; }

        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;

        public string Satuan { get; set; } = string.Empty;
        public string SatuanKonversi { get; set; } = string.Empty;

        public decimal ConversionToBase { get; set; }

        public bool IsBase { get; set; }
        public bool IsDefaultSales { get; set; }
        public bool IsDefaultPurchase { get; set; }

        public string Barcode { get; set; } = string.Empty;
        public bool IsInternalBarcode { get; set; }
    }
}
