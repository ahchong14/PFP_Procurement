using PFP.Host.PFP.Domain.Interface.Auditables;

namespace PFP.Host.PFP.Domain.Interface
{
    public interface IBaseEntity : IEntity, ICreationAuditable
    {
    }
}
