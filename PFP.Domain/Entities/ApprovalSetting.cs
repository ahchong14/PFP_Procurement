using PFP.Domain.Enums;
using PFP.Domain.Interface;

namespace PFP.Domain.Entities
{
    public class ApprovalSetting : IExposableEntity
    {
        public ApprovalLevel level { get; set; }

        public Role ApproverRole { get; set; }

        public decimal MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

    }
}
