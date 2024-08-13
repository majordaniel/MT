using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace UtilityLibrary.Pagination
{
    public class DateRangedPaginationParameter<T> : PaginationParameter<T> where T :  new()
    {
        public DateTime StartDate { get; set; } =  DateTime.Now.AddMonths(-6);
        public DateTime EndDate { get; set; } = DateTime.Now;
    }

    public class PaginationParameter<T> where T :  new()
    {
        protected const int maxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        protected int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }

    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public int Fn { get { return ((CurrentPage - 1) * PageSize); } } //S/N coefficient
    }

    public class PagedList1<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList1(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
            AddRange(items);
        }
        public static PagedList1<T> ToPagedList(IEnumerable<T> source, int count, int pageNumber, int pageSize)
        {
            //var count = source.Count();
            var items = source.ToList();
            return new PagedList1<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList1<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize, string? orderBy = null)
        {
            var props = typeof(T).GetProperties();

            if (props != null)
            {
                if (orderBy == null || !props.Any(c => c.Name == orderBy))
                {
                    var defaultProp = props.FirstOrDefault();
                    if (defaultProp != null)
                    {
                        orderBy = defaultProp.Name;
                    }
                }
            }

            int count;
            List<T> items;

            if (!string.IsNullOrEmpty(orderBy))
            {
                count = source.Count();
                items = source
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize).AsQueryable().OrderBy(orderBy).ToList();
            } else
            {
                count = source.Count();
                items = source
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize).AsQueryable().ToList();
            }

            return new PagedList1<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList1<T> Empty()
        {
            return new PagedList1<T>(new List<T>(), 0, 0, 0);
        }
    }
}
