namespace MTMiddleware.Core.Services.Interfaces
{
    public interface IDashboardService
    {
     
        // Dashboards
        //Customer Dashboards
        Task<Response<CustomerDashboardViewModel>> GetACustomersDashboardDetails(string accountNo);

        //Admin Dashboards
        Task<Response<AdminDashboardViewModel>> GetAdminDashboardDetails();


    }
}
