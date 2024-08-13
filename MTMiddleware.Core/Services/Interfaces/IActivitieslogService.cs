using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.Services.Interfaces
{
    public interface IActivitieslogService
    {
      
        Task<Response<PagedList<ActivitiesLogResponseViewModel>>> 
            GetAllActivitiesLog(DateRangeQueryModel queryModel);
        Task<Response<ActivitiesLogResponseViewModel>> CreateActivityLog(ActivitiesLogRequestViewModel model);

    }
}
