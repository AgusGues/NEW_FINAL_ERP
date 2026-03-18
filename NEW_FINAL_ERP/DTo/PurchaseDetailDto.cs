namespace NEW_FINAL_ERP.DTo
{
    public class PurchaseDetailDto
    {
        public int PurchaseDetailId { get; set; }

        public int ItemId { get; set; }
        public int ItemUOMId { get; set; }
        public int UnitId { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        public decimal BaseQuantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }

        public decimal LineTotal { get; set; }
    }
}