using PFP.Domain.Entities.Commons.Users;
using PFP.Domain.Enums;
using PFP.Domain.Interface;

namespace PFP.Domain.Entities.RequestQuotations
{
    public class RQApproval : IEntity, IExposableEntity
    {
        /// <summary>
        /// The unique identifier for the RQApproval entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The unique identifier for the associated RequestQuotation entity.
        /// </summary>
        public required int RequestQuotationId { get; set; }
        /// <summary>
        /// The associated RequestQuotation entity that this approval belongs to.
        /// </summary>
        public required RequestQuotation RequestQuotation { get; set; }
        /// <summary>
        /// The approval level for this RQApproval entity, indicating the level of approval required.
        /// </summary>
        public ApprovalLevel Level { get; set; }
        /// <summary>
        /// The unique identifier for the user who is the approver for this RQApproval entity.
        /// </summary>
        public int ApproverId { get; set; }
        /// <summary>
        /// The user who is the approver for this RQApproval entity.
        /// </summary>
        public required User Approver { get; set; } = default!;
        /// <summary>
        /// The status of the approval for this RQApproval entity, indicating whether it has been approved, rejected, or is pending.
        /// </summary>
        /// 
        public ApprovalAction Action { get; set; }
        public string? Remark { get; set; }
        /// <summary>
        /// The timestamp indicating when the approval action was taken for this RQApproval entity.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
