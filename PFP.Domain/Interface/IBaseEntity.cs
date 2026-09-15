using PFP.Domain.Interface.Auditables;

namespace PFP.Domain.Interface
{
    public interface IBaseEntity : IEntity, ICreationAuditable
    {
    }
}
