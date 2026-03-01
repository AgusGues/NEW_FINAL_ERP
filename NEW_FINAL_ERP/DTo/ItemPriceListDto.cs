namespace NEW_FINAL_ERP.DTo
{
    public class ItemPriceListDto
    {
        public int ItemPriceId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string PriceType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
