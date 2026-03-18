namespace NEW_FINAL_ERP.Models
{
    public class PurchaseDetail
    {
        public int PurchaseDetailId { get; set; }
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public int ItemUOMId { get; set; }
        public int UnitId { get; set; }

        public decimal Quantity { get; set; }
        public decimal BaseQuantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }

        public decimal LineTotal { get; set; }

        public decimal? AverageCostBefore { get; set; }
        public decimal? AverageCostAfter { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}