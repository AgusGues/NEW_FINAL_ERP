namespace NEW_FINAL_ERP.DTo
{
    public class ItemPriceFormDto
    {
        public int ItemPriceId { get; set; }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
