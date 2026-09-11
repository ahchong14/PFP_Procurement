using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Domain.Entities.Commons.PurchaseOrders
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string DocNo { get; set; }
        public int RequestQuotationId { get; set; }

        public int SupplierId { get; set; }

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




    }
}
