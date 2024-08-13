using MTMiddleware.Core.Auth;
using MTMiddleware.Core.ExternalServices;
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using MTMiddleware.Core.Helpers.EmailSender;
using MTMiddleware.Core.Helpers.EmailSender.Interfaces;

namespace MTMiddleware.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbInit(this IServiceCollection services)
    {
        services.AddTransient<IDbInitializer, DbInitializer>();

        return services;
    }

    public static IServiceCollection AddJwtServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {

        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IActivitieslogService, ActivitieslogService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IUtilityService, UtilityService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailSenderService2, EmailSenderService2>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddHttpClient<IMailerService, MailerService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IExternalAPIServices, ExternalAPIServices>();

        return services;
    }
}