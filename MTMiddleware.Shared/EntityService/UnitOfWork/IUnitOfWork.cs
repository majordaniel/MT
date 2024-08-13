using MTMiddleware.Shared.EntityService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.EntityService.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T, Tkey> GetRepository<T, Tkey>() where T : class;
    IQueryable<T> FromSql<T>(FormattableString sql) where T : class;
    IQueryable<T> FromSqlRaw<T>(string sql, params object[] parameters) where T : class;
    int ExecuteSqlCommand(string sql, params object[] parameters);

    int SaveChanges();
    Task<int> SaveChangesAsync();
}
