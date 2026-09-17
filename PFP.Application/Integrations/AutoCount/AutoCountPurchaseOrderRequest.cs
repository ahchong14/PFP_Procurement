namespace PFP.Application.Integrations.AutoCount
{
    public sealed record AutoCountPurchaseOrderRequest(
    string DocNo,
    string CreditorCode,
    decimal TotalAmount,
    IReadOnlyList<AutoCountPurchaseOrderLineRequest> Items);

    public sealed record AutoCountPurchaseOrderLineRequest(
        string ItemCode,
        string Description,
        string Uom,
        decimal Qty,
        decimal UnitPrice);
}
