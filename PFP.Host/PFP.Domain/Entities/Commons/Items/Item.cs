using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.Items;

public class Item : IBaseEntity, IExposableEntity
{
    public int Id { get; set; }

    public string? DocNo { get; set; }
    public string? Name { get; set; }

    public string? Uom { get; set; } = null;
    public decimal RefPrice { get; set; }
}
