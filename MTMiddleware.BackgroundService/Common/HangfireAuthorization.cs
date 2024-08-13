using Hangfire.Dashboard;
using Hangfire.Annotations;

namespace MTMiddleware.BackgroundService.Common
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
