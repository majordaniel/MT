using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTMiddleware.Core.ExternalServices.Salesforce;
using MTMiddleware.Shared.EntityService.UnitOfWork;
using MTMiddleware.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MTMiddleware.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSalesforceCustomerServices(this IServiceCollection services)
    {
        services.AddScoped<ISalesforceCustomerService, SalesforceCustomerService>();

        return services;
    }
}
