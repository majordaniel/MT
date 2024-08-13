namespace MTMiddleware.Core.Services.Interfaces
{
    public interface IUserService
    {




        //Users
        Task<Response<ApplicationUserViewModel>> InviteAsync(InviteUserViewModel model, DateTime currentDateTime);
        Task<Response<SignInViewModel>> SignInAsync(AuthCredentialViewModel model, DateTime currentDateTime);
        Task<Response<ApplicationUserViewModel>> UpdateUserAsync(Guid id, UpdateUserViewModel model, DateTime currentDateTime);

        Task<Response<ApplicationUserViewModel>> GetUserAsync(Guid id);
        Task<Response<ApplicationUserViewModel>> EnableAsync(Guid id);
        Task<Response<ApplicationUserViewModel>> DisableAsync(Guid id);
        Task<Response<List<ApplicationUserViewModel>>> GetUsersAsync();
        Task<Response<List<ApplicationUserViewModel>>> GetApproversUsersAsync();
        Task<Response<List<ApplicationUserViewModel>>> GetSuperAdminUsersAsync();


        Task<Response<List<ApplicationUserViewModel>>> GetCustomerUsersAsync();
        Task<Response<PagedList<ApplicationUserViewModel>>> GetCustomerAllUsersAsync(QueryModel queryModel);
        Task<Response<PagedList<ApplicationUserViewModel>>> GetAllcustomerUsersByStatus(QueryModel queryModel, bool isactive);
        
        
        Task<Response<bool>> RequestPasswordResetAsync(EmailViewModel model, DateTime currentDateTime);
        Task<Response<bool>> ResetPasswordAsync(ResetPasswordViewModel model, DateTime currentDateTime);
        Task<Response<bool>> DeleteAsync(Guid id, DateTime currentDateTime);
        Task<Response<PagedList<ApplicationUserViewModel>>> SearchAsync(QueryModel queryModel, DateTime currentDateTime);
        Task<Response<List<ApplicationUserViewModel>>> GetAllUsers(ExportQueryModel queryModel);
        Task<Response<bool>> ChangeUserRole(ChangeUserRoleViewModel model);


    }
}
