using PFP.Application.Common.Results;

namespace PFP.Application.Integrations.AutoCount;

public interface IAutoCountService
{
    Task<List<AutoCountItemDto>> PullItemsAsync(CancellationToken cancellationToken);
    Task<List<AutoCountSupplierDto>> PullSuppliersAsync(CancellationToken cancellationToken);
    Task<Result<string>> PushPurchaseOrderAsync(AutoCountPurchaseOrderRequest request, CancellationToken cancellationToken);
    Task<Result<string>> PushNewCreditorAsync(AutoCountCreditorRequest request, CancellationToken cancellationToken);
}
