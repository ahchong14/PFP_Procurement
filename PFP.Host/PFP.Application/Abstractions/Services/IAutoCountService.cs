using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Entities.PurchaseOrders;

namespace PFP.Host.PFP.Application.Abstractions.Services
{
    public interface IAutoCountService
    {
        Task<List<AutoCountItemDto>> PullItemsAsync(CancellationToken cancellationToken);
        Task<List<AutoCountSupplierDto>> PullSuppliersAsync(CancellationToken cancellationToken);
        Task<string> PushPurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken);
        Task<string> PushNewCreditorAsync(Supplier supplier, CancellationToken cancellationToken);
    }
}

public sealed record AutoCountItemDto(string Code, string Name, string Uom, decimal RefPrice);
public sealed record AutoCountSupplierDto(string CreditorCode, string Name, string? Contact, string Email);
