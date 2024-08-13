using MTMiddleware.Data.ViewModels;
using UtilityLibrary.Models;

namespace MTMiddleware.Core.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Response<ApplicationRoleViewModel>> GetItemAsync(Guid id);
        Task<Response<List<ApplicationRoleViewModel>>> GetRolesAsync();
        Task<Response<List<ApplicationRoleViewModel>>> ExportDataAsync(ExportQueryModel queryModel);
    }
}
