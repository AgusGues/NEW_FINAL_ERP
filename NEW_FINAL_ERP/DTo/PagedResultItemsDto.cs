namespace NEW_FINAL_ERP.DTo
{
    public class PagedResultItemsDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalData { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
    }
}
