using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequestDetails;
using PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteCopys;
using PFP.Host.PFP.Domain.Entities.Commons.Users;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;


namespace PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests
{
    public class PurchaseRequest : IBaseEntity, IBaseExposableEntity
    {
        public int Id { get; set; }

        public string DocNo { get; set; } = string.Empty;

        public required int RequestedId { get; set; }

        public required User User { get; set; } = default!;

        public Department Department { get; set; } = Department.Office;

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

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public ICollection<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();

        public ICollection<SupplierQuoteCopy> SupplierQuoteCopies { get; set; } = new List<SupplierQuoteCopy>();
    }
}
