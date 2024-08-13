using MTMiddleware.Shared.Abstracts;
using MTMiddleware.Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Data.ViewModels;

public class DateRangeViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}