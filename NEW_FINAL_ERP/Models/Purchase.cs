namespace NEW_FINAL_ERP.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public string CurrencyCode { get; set; } = "IDR";
        public decimal ExchangeRate { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherCost { get; set; }
        public decimal GrandTotal { get; set; }

        public string Status { get; set; } = "Draft";
        public string? Remarks { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}