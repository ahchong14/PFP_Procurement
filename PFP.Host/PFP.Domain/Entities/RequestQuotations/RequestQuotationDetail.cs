using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.RequestQuotations

{
    public class RequestQuotationDetail : IEntity, IExposableEntity
    {
        public int Id { get; set; }

        public required int RequestQuotationId { get; set; }

        public required RequestQuotation RequestQuotation { get; set; }

        public required string ItemCode { get; set; }
        public required string Description { get; set; }

        public required string Uom { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
