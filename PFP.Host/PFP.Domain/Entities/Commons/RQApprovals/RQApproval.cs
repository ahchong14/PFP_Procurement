using PFP.Host.PFP.Domain.Entities.Commons.RequestQuotations;
using PFP.Host.PFP.Domain.Entities.Commons.Users;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.RQApprovals
{
    public class RQApproval : IBaseEntity, IBaseExposableEntity
    {
        public int Id { get; set; }

        public required int RequestQuotationId { get; set; }

        public required RequestQuotation RequestQuotation { get; set; }

        public ApprovalLevel Level { get; set; }

        public int ApproverId { get; set; }

        public required User User { get; set; } = default!;

        public string? Remark { get; set; }

        public DateTime Timestamp { get; set; }


    }
}
