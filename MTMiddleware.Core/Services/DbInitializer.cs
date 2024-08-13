using MTMiddleware.Core.DbSeeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core;

public class DbInitializer : IDbInitializer
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DbInitializer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void Initialize()
    {
        using (var serviceScope = _scopeFactory.CreateScope())
        {
            using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
            {
                if (context != null)
                {
                    //context.Database.EnsureCreated();
                    context.Database.Migrate();
                }
            }
        }
    }

    public void SeedData()
    {
        try
        {


            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {
                    if (context != null)
                    {
                        //AnnualSalaryRangeSeeder.Execute(context);
                        //AnnualTurnoverSeeder.Execute(context);
                        //BankInterestRateSeeder.Execute(context);
                        //DigitalChannelSeeder.Execute(context);
                        //EmploymentStatusSeeder.Execute(context);
                        //FamilyRelationshipSeeder.Execute(context);
                        //GenderSeeder.Execute(context);
                        //MaritalStatusSeeder.Execute(context);
                        //MeansOfIdentificationSeeder.Execute(context);
                        //ResidencyStatusSeeder.Execute(context);
                        //SignatoryClassSeeder.Execute(context);
                        //TenorSeeder.Execute(context);
                        //TitleSeeder.Execute(context);
                        //CountrySeeder.Execute(context);
                        //BusinessOptionSeeder.Execute(context);
                        RoleSeeder.Execute(context);
                    }
                }
            }

        }
        catch (Exception ex)
        {

            throw;
        }

    }
}
