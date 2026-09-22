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
using PFP.Infrastructure.Persistence.Repositories;

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
        AddEmail(services, configuration);
        AddDocumentNumbers(services);
        AddAutoCount(services, configuration);

        return services;
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
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.SectionName));

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
