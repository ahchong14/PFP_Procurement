using PFP.Domain.Entities.Commons.Users;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Domain.Entities.SupplierQuoteCopys;
using PFP.Domain.Enums;
using PFP.Domain.Interface;


namespace PFP.Domain.Entities.PurchaseRequests
{
    public class PurchaseRequest : IBaseEntity, IBaseExposableEntity
    {
        public int Id { get; set; }

        public string DocNo { get; set; } = string.Empty;

        public required int RequesterId { get; set; }

        public required User Requester { get; set; } = default!;

        public string Department { get; set; } = null!;
        public PRStatus Status { get; set; } = PRStatus.Quoting;

        public string? PmRemarks { get; set; }

        public int? SelectedSupplierCopyId { get; set; }

        public SupplierQuoteCopy? SelectedSupplierCopy { get; set; } = default!;

        public string? CreditorCode { get; set; } = string.Empty;

        public string? CreditorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime DecidedAt { get; set; }

        public int? DecidedByUserId { get; set; }

        public User? DecidedByUser { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public ICollection<PurchaseRequestDetail> Items { get; set; } = [];

        public ICollection<SupplierQuoteCopy> SupplierQuoteCopies { get; set; } = [];

        public ICollection<RequestQuotation> RequestQuotations { get; set; } = [];
    }
}
