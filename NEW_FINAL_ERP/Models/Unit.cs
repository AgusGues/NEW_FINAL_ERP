namespace NEW_FINAL_ERP.Models
{
    public class Unit
    {
        public int UnitId { get; set; }
        public string? UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Description { get; set; }

        public int IsActive { get; set; }
    }
}
