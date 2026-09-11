using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequestDetails;
using PFP.Host.PFP.Domain.Entities.Commons.RequestQuotations;
using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.PurchaseOrders
{
    public class PurchaseOrder : IBaseEntity, IBaseExposableEntity
    {
        public int Id { get; set; }
        public required string DocNo { get; set; }
        public required int RequestQuotationId { get; set; }

        public required RequestQuotation RequestQuotation { get; set; }

        public required int SupplierId { get; set; }

        public required Supplier Supplier { get; set; } = default!;

        public bool IsNewSupplier { get; set; }

        public decimal TotalAmount { get; set; }

        public POStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SyncedAt { get; set; }

        public string? AutoCountPORRef { get; set; }

        public string? AutoCountCreditorRef { get; set; }

        public string? SyncError { get; set; }

        public int SyncAttempts { get; set; }

        public required byte[] RowVersion { get; set; }

        public ICollection<PurchaseRequestDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseRequestDetail>();

        public void MarkAsSynced(string poRef, string creditorRef)
        {
            if (string.IsNullOrWhiteSpace(poRef))
            {
                throw new ArgumentException("AutoCount PO reference cannot be null or empty.", nameof(poRef));
            }
            if (string.IsNullOrWhiteSpace(creditorRef))
            {
                throw new ArgumentException("AutoCount Creditor reference cannot be null or empty.", nameof(creditorRef));
            }
            AutoCountPORRef = poRef;
            AutoCountCreditorRef = creditorRef;
            Status = POStatus.Synced;
            SyncedAt = DateTime.UtcNow;
        }
    }
}
