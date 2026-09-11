namespace PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteDetails
{
    public class SupplierQuoteDetail
    {
        public int Id { get; set; }

        public int SupplierQuoteCopyId { get; set; } = 0;

        public int PurchaseRequestItemId { get; set; } = 0;

        public decimal UnitPrice { get; set; } = 0;
    }
}
