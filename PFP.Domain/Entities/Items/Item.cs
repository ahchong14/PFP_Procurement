using PFP.Domain.Interface;

namespace PFP.Domain.Entities.Items;

public class Item : IEntity, IExposableEntity
{
    public int Id { get; set; }

    public required string Code { get; set; }
    public required string Name { get; set; }

    public required string Uom { get; set; } = null!;
    public decimal RefPrice { get; set; }
}
