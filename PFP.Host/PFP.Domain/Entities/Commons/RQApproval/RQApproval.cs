using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Domain.Entities.Commons.RQApproval
{
    public class RQApproval
    {
        public int Id { get; set; }

        public int RequestQuotationId { get; set; }

        public ApprovalLevel Level { get; set; }

        public int ApproverId { get; set; }

        public string? Remark { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
