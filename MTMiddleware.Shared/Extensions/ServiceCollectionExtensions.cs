using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTMiddleware.Shared.EntityService.UnitOfWork;
using MTMiddleware.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace MTMiddleware.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();

        return services;
    }

    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IApiCaller, ApiCaller>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpContextAccessor();

        return services;
    }
}
