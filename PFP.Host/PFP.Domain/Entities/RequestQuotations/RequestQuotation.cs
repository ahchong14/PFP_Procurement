using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Entities.PurchaseOrders;
using PFP.Host.PFP.Domain.Entities.PurchaseRequests;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.RequestQuotations
{
    public class RequestQuotation : IBaseEntity, IBaseExposableEntity

    {

        /// <summary>
        /// The unique identifier for the RequestQuotation entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// the unique document number for the RequestQuotation entity.
        /// </summary>
        public string DocNo { get; set; } = string.Empty;
        /// <summary>
        /// The unique identifier for the associated PurchaseRequest entity.
        /// </summary>
        public int PurchaseRequestId { get; set; } = 0;
        /// <summary>
        /// The associated PurchaseRequest entity for the RequestQuotation.
        /// </summary>
        public required PurchaseRequest PurchaseRequest { get; set; } = default!;
        /// <summary>
        /// The unique identifier for the associated Supplier entity.
        /// </summary>
        public int SupplierId { get; set; } = 0;
        /// <summary>
        /// The associated Supplier entity for the RequestQuotation.
        /// </summary>
        public required Supplier Supplier { get; set; } = default!;
        /// <summary>
        /// The total amount for the RequestQuotation entity, representing the sum of all quoted items.
        /// </summary>
        public decimal TotalAmount { get; set; } = 0;
        /// <summary>
        /// The status of the RequestQuotation entity, indicating its current state in the workflow.
        /// </summary>
        public RQStatus Status { get; set; }
        /// <summary>
        /// Indicates whether the RequestQuotation requires Level 2 approval. If true, the quotation must go through an additional approval step before finalization.
        /// </summary>
        public bool RequiresL2 { get; set; }
        /// <summary>
        /// The date and time when the RequestQuotation entity was created. This property is automatically set to the current date and time when a new RequestQuotation is instantiated.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// The date and time when the RequestQuotation entity was last updated. This property is automatically set to the current date and time whenever the RequestQuotation is modified.
        /// </summary>
        public required byte[] RowVersion { get; set; }
        /// <summary>
        /// A collection of RequestQuotationDetail entities associated with the RequestQuotation. Each RequestQuotationDetail represents an individual item or service being quoted, including its description, quantity, unit price, and total price. This collection allows for detailed tracking of all items included in the quotation.
        /// </summary>
        public ICollection<RequestQuotationDetail> Items { get; set; } = [];
        /// <summary>
        /// A collection of RQApproval entities associated with the RequestQuotation. Each RQApproval represents an approval action taken on the quotation, including the approver's information, approval status, and any comments provided. This collection allows for tracking the approval history and workflow of the quotation.
        /// </summary>
        public ICollection<RQApproval> Approvals { get; set; } = [];
        /// <summary>
        /// A c
        /// </summary>
        public ICollection<PurchaseOrder> PurchaseOrder { get; set; } = [];
    }
}
