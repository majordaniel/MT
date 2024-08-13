namespace MTMiddleware.Core.Services.Interfaces
{
    public interface ICustomerService
    {


        Task<Response<ApplicationUserViewModel>> RegisterCustomerAsync(RegisterCustomerRequestViewModel model, DateTime currentDateTime);
        Task<Response<ApplicationUserViewModel>> UpdateCustomerApprovalAndLimit(UpdateCustomerApprovalAndLimit model);
        //Customer details
        Task<Response<PagedList<CustomerDetailsResponseViewModel>>> GetAllPendingCustomersDetails(QueryModel queryModel);
        Task<Response<PagedList<CustomerDetailsResponseViewModel>>> GetAllCustomersDetails(QueryModel queryModel);
        Task<Response<CustomerDetailsResponseViewModel>> GetACustomersDetails(Guid id);
        Task<Response<bool>> ChangeCustomerStatus(ChangeCustomerStatusViewModel model);



    }
}
