namespace NEW_FINAL_ERP.DTo
{
    public class BussinesPatnerListDto
    {
        public int BusinessPartnerId { get; set; }
        public string BPCode { get; set; }
        public string BPName { get; set; }
        public string BPType { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CreditLimit { get; set; }
        public string PaymentTerm { get; set; }
        public string TaxNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
