using PFP.Host.PFP.Domain.Entities.Commons.PurchaseOrders;
using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.Commons.RequestQuotationDetails;
using PFP.Host.PFP.Domain.Entities.Commons.RQApprovals;
using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.RequestQuotations
{
    public class RequestQuotation : IBaseEntity, IBaseExposableEntity

    {

        /// <summary>
        /// get or set the RequestQuotation Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// get or set the RequestQuotation DocNo
        /// </summary>
        public string DocNo { get; set; } = string.Empty;
        /// <summary>
        /// get or set the RequestQuotation PurchaseRequestId
        /// </summary>
        public int PurchaseRequestId { get; set; } = 0;

        public required PurchaseRequest PurchaseRequest { get; set; } = default!;
        /// <summary>
        /// get or set the RequestQuotation SupplierId
        /// </summary>
        public int SupplierId { get; set; } = 0;

        public required Supplier Supplier { get; set; } = default!;
        /// <summary>
        /// get or set the RequestQuotation TotalAmount 
        /// </summary>
        public decimal TotalAmount { get; set; } = 0;
        /// <summary>
        /// get or set the RequestQuotation Status
        /// </summary>
        public RQStatus Status { get; set; }
        /// <summary>
        /// get or set the RequestQuotation RequiresL2
        /// </summary>
        public bool RequiresL2 { get; set; }
        /// <summary>
        /// get or set the RequestQuotation Createdat
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// get or set the RequestQuotation SyncedAt
        /// </summary>
        public required byte[] RowVersion { get; set; }

        public ICollection<RequestQuotationDetail> RequestQuotationDetails { get; set; } = new List<RequestQuotationDetail>();

        public ICollection<RQApproval> RQApprovals { get; set; } = new List<RQApproval>();

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
