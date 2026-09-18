using Microsoft.EntityFrameworkCore;

using PFP.Domain.Entities;
using PFP.Domain.Entities.Commons.Suppliers;
using PFP.Domain.Entities.Commons.Users;
using PFP.Domain.Entities.Items;
using PFP.Domain.Entities.PurchaseOrders;
using PFP.Domain.Entities.PurchaseRequests;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Domain.Entities.SupplierQuoteCopys;

namespace PFP.Infrastructure.Persistence.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();

    public DbSet<PurchaseRequestDetail> PurchaseRequestDetails => Set<PurchaseRequestDetail>();

    public DbSet<SupplierQuoteCopy> SupplierQuoteCopies => Set<SupplierQuoteCopy>();

    public DbSet<SupplierQuoteDetail> SupplierQuoteDetails => Set<SupplierQuoteDetail>();

    public DbSet<RequestQuotation> RequestQuotations => Set<RequestQuotation>();

    public DbSet<RequestQuotationDetail> RequestQuotationDetails => Set<RequestQuotationDetail>();

    public DbSet<RQApproval> RQApprovals => Set<RQApproval>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();

    public DbSet<ApprovalSetting> ApprovalSettings => Set<ApprovalSetting>();

    public DbSet<Counter> Counters => Set<Counter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}

