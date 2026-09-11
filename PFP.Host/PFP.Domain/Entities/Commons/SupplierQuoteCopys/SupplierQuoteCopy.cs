using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteDetails;
using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteCopys
{
    public class SupplierQuoteCopy : IBaseEntity, IBaseExposableEntity
    {
        public int Id { get; set; }

        public required int PurchaseRequestId { get; set; } = 0;

        public required PurchaseRequest PurchaseRequest { get; set; } = default!;

        public required int SupplierId { get; set; } = 0;

        public required Supplier Supplier { get; set; } = default!;

        public string Token { get; set; } = string.Empty;

        public Copystatus Status { get; set; } = Copystatus.PendingL1;

        public string? Remarks { get; set; }

        public decimal TotalAmount { get; set; } = 0m;

        public DateTime SentAt { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public void SubmitQuote(decimal amount, string? remarks = null)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            TotalAmount = amount;
            Remarks = remarks;
            Status = Copystatus.Converted;
            SubmittedAt = DateTime.UtcNow;

        }

        public ICollection<SupplierQuoteDetail> SupplierQuoteDetails { get; set; } = new List<SupplierQuoteDetail>();
    }
}
