using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;

namespace PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequestDetails
{
    public class PurchaseRequestDetail
    {
        public int Id { get; set; }

        public int PurchaseRequestId { get; set; } = 0;

        public PurchaseRequest PurchaseRequest { get; set; } = null!;

        public string ItemCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string Uom { get; set; } = string.Empty;

        public decimal Qty { get; set; } = 0;
    }
}
