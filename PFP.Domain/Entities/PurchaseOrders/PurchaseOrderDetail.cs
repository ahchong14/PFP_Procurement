using PFP.Domain.Interface;

namespace PFP.Domain.Entities.PurchaseOrders
{
    public class PurchaseOrderDetail : IEntity, IExposableEntity
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = default!;
        public string ItemCode { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Uom { get; set; } = default!;
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
