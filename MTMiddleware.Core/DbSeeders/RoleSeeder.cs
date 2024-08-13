using MTMiddleware.Data;
using MTMiddleware.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.DbSeeders;

public static class RoleSeeder
{
    public static void Execute(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            DateTime currentDate = DateTime.Now;

            var item = new ApplicationRole
            {
                Name = "SuperAdmin",
                IsSystemRole = true,
                DateCreated = currentDate,
                DateLastUpdated = currentDate
            };
            context.Roles.Add(item);

            item = new ApplicationRole
            {
                Name = "Approver",
                IsSystemRole = false,
                DateCreated = currentDate,
                DateLastUpdated = currentDate
            };
            context.Roles.Add(item);

            item = new ApplicationRole
            {
                Name = "Customer",
                IsSystemRole = false,
                DateCreated = currentDate,
                DateLastUpdated = currentDate
            };
            context.Roles.Add(item);

            context.SaveChanges();
        }
    }
}
