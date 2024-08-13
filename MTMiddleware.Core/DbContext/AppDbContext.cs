
using MTMiddleware.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTMiddleware.Core;

public partial class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFCoreConfigurationEntrypoint).Assembly);
    }


    public DbSet<CustomerDetails> CustomerDetails { get; set; }
    public DbSet<CustomerAccounts> CustomerAccounts { get; set; }
    public DbSet<ActivitiesLog> ActivitiesLog { get; set; }
    public DbSet<CustomersChannelTransKey> CustomersChannelTransKey { get; set; }
    public DbSet<CustomerTransactions> CustomerTransactions { get; set; }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
