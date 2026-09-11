namespace PFP.Host.PFP.Domain.Interface.Auditables
{
    public interface IConcurrencyAuditable
    {
        byte[] RowVersion { get; set; }
    }
}
