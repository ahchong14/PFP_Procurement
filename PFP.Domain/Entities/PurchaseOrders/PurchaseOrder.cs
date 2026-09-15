using PFP.Domain.Entities.Commons.Suppliers;
using PFP.Domain.Entities.PurchaseRequests;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Domain.Enums;
using PFP.Domain.Interface;
using PFP.Domain.Interface.Concurrency;

namespace PFP.Domain.Entities.PurchaseOrders
{
    public class PurchaseOrder : IBaseExposableEntity, IConcurrencyAware
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

        public string? AutoCountPORef { get; set; }

        public string? AutoCountCreditorRef { get; set; }

        public string? SyncError { get; set; }

        public int SyncAttempts { get; set; }

        public required byte[] RowVersion { get; set; } = [];

        public ICollection<PurchaseRequestDetail> Items { get; set; } = [];
    }
}
