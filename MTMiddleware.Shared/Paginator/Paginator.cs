using Microsoft.EntityFrameworkCore;
using UtilityLibrary.Models;

namespace MTMiddleware.Shared.Pagination
{
    public class Paginator<T> : IPaginator<T>
    {
        private const int DEFAULT_PAGE_SIZE = 10;

        public List<T> Paginate(IQueryable<T> query, BaseQueryModel model)
        {
            var pageIndex = model.PageNumber < 1 ? 1 : model.PageNumber;
            var pageSize = model.PageSize < 1 ? DEFAULT_PAGE_SIZE : model.PageSize;

            var data =  query.Paginate(pageIndex, pageSize).ToList();

            return data;
        }

        public async Task<List<T>> PaginateAsync(IQueryable<T> query, BaseQueryModel model)
        {
            var pageIndex = model.PageNumber < 1 ? 1 : model.PageNumber;
            var pageSize = model.PageSize < 1 ? DEFAULT_PAGE_SIZE : model.PageSize;

            var data = await query.Paginate(pageIndex, pageSize).ToListAsync()  ;

            return data;
        }
    }
}
