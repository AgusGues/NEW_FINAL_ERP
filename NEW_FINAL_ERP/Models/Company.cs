using System.ComponentModel.DataAnnotations;

namespace NEW_FINAL_ERP.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string? CompanyCode { get; set; }

        [Required(ErrorMessage = "Company name wajib diisi")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public string BaseCurrencyCode { get; set; }

        public bool IsActive { get; set; } = true;
    }
}