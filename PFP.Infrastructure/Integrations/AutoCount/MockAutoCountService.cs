using PFP.Application.Common.Results;
using PFP.Application.Integrations.AutoCount;

namespace PFP.Infrastructure.Integrations.AutoCount;

// Stand-in for AutoCountService until the real AutoCount HTTP API (endpoints, auth,
// request/response shapes) is known. Registered in DependencyInjection so local
// development and testing can run end-to-end without a live AutoCount connection.
internal sealed class MockAutoCountService : IAutoCountService
{
    public Task<List<AutoCountItemDto>> PullItemsAsync(CancellationToken cancellationToken)
        => Task.FromResult(new List<AutoCountItemDto>());

    public Task<List<AutoCountSupplierDto>> PullSuppliersAsync(CancellationToken cancellationToken)
        => Task.FromResult(new List<AutoCountSupplierDto>());

    public Task<Result<string>> PushPurchaseOrderAsync(
        AutoCountPurchaseOrderRequest request,
        CancellationToken cancellationToken)
        => Task.FromResult(Result<string>.Success($"MOCK-{request.DocNo}"));

    public Task<Result<string>> PushNewCreditorAsync(
        AutoCountCreditorRequest request,
        CancellationToken cancellationToken)
        => Task.FromResult(Result<string>.Success($"MOCK-CREDITOR-{request.Name}"));
}
