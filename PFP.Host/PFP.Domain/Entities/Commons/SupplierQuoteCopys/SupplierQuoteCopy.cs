using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteCopys
{
    public class SupplierQuoteCopy
    {
        public int Id { get; set; }

        public int PurchaseRequestId { get; set; } = 0;

        public int SupplierId { get; set; } = 0;

        public Supplier Supplier { get; set; } = null!;

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
    }
}
