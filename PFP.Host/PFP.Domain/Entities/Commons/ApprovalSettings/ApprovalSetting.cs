using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Domain.Entities.Commons.ApprovalSettings
{
    public class ApprovalSetting
    {
        public ApprovalLevel level { get; set; }

        public Role ApproverRole { get; set; }

        public decimal MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

    }
}
