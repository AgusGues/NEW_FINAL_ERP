using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.DTo
{
    public class PurchaseFormDto
    {
        public int PurchaseId { get; set; }
        public string? PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public string CurrencyCode { get; set; } = "IDR";
        public decimal ExchangeRate { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherCost { get; set; }

        public string? Remarks { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<PurchaseDetail> Details { get; set; } = new();
    }
}