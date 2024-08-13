using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.EntityService.Repository;

public interface IRepository<T, Tkey> where T : class
{
    DbSet<T> Entities();

    IQueryable<T> GetAll();
    T GetItem(Tkey id);

    IEnumerable<T> GetItems(Func<T, bool> predicate, string navigation);

    IEnumerable<T> GetItems(Func<T, bool> predicate);

    void Add(T entity);
    void SaveChanges();
    ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
    void AddRange(IEnumerable<T> entities);

    void Remove(Tkey id);
    void Remove(T entity);

    int Update(Tkey id, T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> selector);
}
