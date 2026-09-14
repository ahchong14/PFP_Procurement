using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities
{
    public class ApprovalSetting : IExposableEntity
    {
        public ApprovalLevel level { get; set; }

        public Role ApproverRole { get; set; }

        public decimal MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

    }
}
