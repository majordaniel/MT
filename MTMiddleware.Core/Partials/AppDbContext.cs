
using System;
using System.Collections.Generic;
using System.Text;
using MTMiddleware.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Serilog;

namespace MTMiddleware.Core;

public partial class AppDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<InvestmentStatusReportViewModel>().HasNoKey().ToView(null);

        //modelBuilder.Entity<ApplicationUser>()
        //.HasOne(e => e.ApplicationRole)
        //.WithOne(e => e.ApplicationUser)
        //.HasForeignKey<ApplicationRole>(e => e.Id)
        //.IsRequired();

       
    }
}
