namespace NEW_FINAL_ERP.DTo
{
    public class PurchaseListDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal GrandTotal { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}