namespace NEW_FINAL_ERP.Models
{
    public class Items
    {
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public int UnitId {  get; set; }
        public string? UnitName { get; set; }
        public int isActive { get; set; }
    }
}
