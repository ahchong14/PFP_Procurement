namespace PFP.Host.PFP.Domain.Interface.Concurrency
{
    public interface IConcrrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
