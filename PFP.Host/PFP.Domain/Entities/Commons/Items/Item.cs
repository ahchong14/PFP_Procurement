namespace PFP.Host.PFP.Domain.Entities.Commons.Items
{
    public class Item
    {
        public int Id { get; set; }

        public string? DocNo { get; set; }
        public string? Name { get; set; }

        public string? Uom { get; set; } = null;
        public decimal RefPrice { get; set; }
    }
}
