namespace PFP.Domain.Interface.Concurrency
{
    public interface IConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
