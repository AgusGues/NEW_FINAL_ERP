using System;

namespace NEW_FINAL_ERP.Models
{
    public class NumberSequence
    {
        public int SequenceId { get; set; }
        public int CompanyId { get; set; }
        public string EntityName { get; set; }
        public string Prefix { get; set; }
        public int NumberLength { get; set; }
        public string? ResetType { get; set; }
        public int VersionNo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
