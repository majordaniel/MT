using MTMiddleware.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTMiddleware.Api;

public static class DbInit
{
    public static void Run(IApplicationBuilder app)
    {
        SeedData(app);
    }

    private static void SeedData(IApplicationBuilder app)
    {
        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            if (dbInitializer != null)
            {
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }
        }
    }
}


