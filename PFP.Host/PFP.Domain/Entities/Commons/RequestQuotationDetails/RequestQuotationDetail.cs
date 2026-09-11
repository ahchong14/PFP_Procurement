namespace PFP.Host.PFP.Domain.Entities.Commons.RequestQuotations
{
    public class RequestQuotation
    {
        public int Id { get; set; }

        public string DocNo { get; set; } = string.Empty;

        public int PurchaseRequestId { get; set; } = 0;

        public int SupplierId { get; set; } = 0;
    }
}
