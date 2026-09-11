using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.Counters
{
    public class Counter : IExposableEntity, IBaseEntity, IBaseExposableEntity
    {
        public string Name { get; set; }

        public int Seq { get; set; }

    }
}
