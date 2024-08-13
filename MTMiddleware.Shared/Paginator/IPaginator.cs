﻿
using UtilityLibrary.Models;

namespace MTMiddleware.Shared.Pagination
{
    public interface IPaginator<T>
    {
        List<T> Paginate(IQueryable<T> query, BaseQueryModel model);
        Task<List<T>> PaginateAsync(IQueryable<T> query, BaseQueryModel model);
    }
}
