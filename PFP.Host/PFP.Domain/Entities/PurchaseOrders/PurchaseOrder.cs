using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Entities.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.RequestQuotations;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;
using PFP.Host.PFP.Domain.Interface.Concurrency;

namespace PFP.Host.PFP.Domain.Entities.PurchaseOrders
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

        public string? AutoCountPORRef { get; set; }

        public string? AutoCountCreditorRef { get; set; }

        public string? SyncError { get; set; }

        public int SyncAttempts { get; set; }

        public required byte[] RowVersion { get; set; } = [];

        public ICollection<PurchaseRequestDetail> Items { get; set; } = [];
    }
}
