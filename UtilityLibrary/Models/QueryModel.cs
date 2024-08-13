namespace UtilityLibrary.Models;

public class BaseQueryModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class QueryModel : BaseQueryModel
{
    public string? Filter { get; set; }
    public string? Keyword { get; set; }
    //public bool? IsActive { get; set; }
}

public class DateRangeQueryModel : QueryModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class YearMonthQueryModel
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class DateRangeQueryXModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ExportQueryModel : DateRangeQueryXModel
{
    public string? Filter { get; set; }
    public string? Keyword { get; set; }
}

public class ExportYearMonthQueryModel : YearMonthQueryModel
{
    public string? Filter { get; set; }
    public string? Keyword { get; set; }
    public int Tenor { get; set; }
}

public class ExportTypeQueryModel : ExportQueryModel
{
    public int Type { get; set; }
}



