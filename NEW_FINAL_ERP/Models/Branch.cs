using System.ComponentModel.DataAnnotations;

namespace NEW_FINAL_ERP.Models
{
    public class Branch
    {
        public int BranchId { get; set; }

        public string? BranchCode { get; set; }

        [Required(ErrorMessage = "Branch name wajib")]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public int VersionNo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}