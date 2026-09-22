using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Abstractions.Services;
using PFP.Application.Integrations.AutoCount;
using PFP.Infrastructure.Email;
using PFP.Infrastructure.Identity;
using PFP.Infrastructure.Integrations.AutoCount;
using PFP.Infrastructure.Options;
using PFP.Infrastructure.Persistence;
using PFP.Infrastructure.Persistence.Database;
using PFP.Infrastructure.Persistence.DocumentNumbers;
using PFP.Infrastructure.Persistence.Database.Seed;
using PFP.Infrastructure.Persistence.Repositories;
using PFP.Infrastructure.Security;

namespace PFP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddRepositories(services);
        AddIdentity(services, configuration);
        AddEmail(services);
        AddDocumentNumbers(services);
        AddAutoCount(services, configuration);

        return services;
    }

    // Called once at startup from Program.cs, after the DI container is built - not part
    // of AddInfrastructureServices itself, since seeding needs a live ApplicationDbContext
    // and ISecretProtector instance, not just their registrations.
    public static async Task SeedInfrastructureAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var secretProtector = scope.ServiceProvider.GetRequiredService<ISecretProtector>();

        await ApplicationDbContextSeed.SeedAsync(dbContext, secretProtector, cancellationToken);
    }

    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    private static void AddRepositories(
        IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IApprovalSettingRepository, ApprovalSettingRepository>();
        services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
        services.AddScoped<ISupplierQuoteRepository, SupplierQuoteRepository>();
        services.AddScoped<IRequestQuotationRepository, RequestQuotationRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailSettingsRepository, EmailSettingsRepository>();
    }

    private static void AddIdentity(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITokenService, JwtTokenService>();
    }

    private static void AddEmail(
        IServiceCollection services)
    {
        // SMTP settings live in the database (see EmailSettings), editable at runtime
        // through the settings page - not static configuration. Data Protection secures
        // the stored password; see DataProtectionSecretProtector for the key-ring caveat.
        services.AddDataProtection();
        services.AddSingleton<ISecretProtector, DataProtectionSecretProtector>();

        services.AddScoped<IEmailService, SmtpEmailService>();
    }

    private static void AddDocumentNumbers(
        IServiceCollection services)
    {
        services.AddScoped<IDocumentNumberGenerator, SequentialDocumentNumberGenerator>();
    }

    private static void AddAutoCount(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AutoCountApiOptions>(
            configuration.GetSection(AutoCountApiOptions.SectionName));

        // TODO: swap to AddHttpClient<IAutoCountService, AutoCountService>() once
        // AutoCount's real endpoints/auth are implemented in AutoCountService.
        // MockAutoCountService keeps every flow that depends on IAutoCountService
        // runnable locally without a live AutoCount connection in the meantime.
        services.AddScoped<IAutoCountService, MockAutoCountService>();
    }
}
