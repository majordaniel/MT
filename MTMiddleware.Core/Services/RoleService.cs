using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTMiddleware.Shared.EntityService.BaseService;
using MTMiddleware.Data.Entities;
using MTMiddleware.Shared.EntityService.UnitOfWork;
using MTMiddleware.Shared.Models;
using MTMiddleware.Data.ViewModels;
using UtilityLibrary.Models;
using UtilityLibrary.Enumerations;
using UtilityLibrary.Extensions;
using MTMiddleware.Core.Services.Interfaces;

namespace MTMiddleware.Core.Services
{
    public class RoleService : BaseService<ApplicationRole, Guid>, IRoleService
    {
        private readonly ILogger<RoleService> _logger;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppSettings _appSettings;

        public RoleService(ILogger<RoleService> logger, IUnitOfWork unitOfWork, IMapper mapper, RoleManager<ApplicationRole> roleManager, IOptions<AppSettings> appSettings) : base(unitOfWork)
        {
            _logger = logger;

            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _roleManager = roleManager;

            _appSettings = appSettings.Value;
        }

        public async Task<Response<List<ApplicationRoleViewModel>>> ExportDataAsync(ExportQueryModel queryModel)
        {
            IQueryable<ApplicationRole> tempQuery;

            tempQuery = GetAll();

            if (queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
            {
                tempQuery = tempQuery
                   .Where(x => x.DateCreated.Date >= queryModel.StartDate.Value && x.DateCreated.Date <= queryModel.EndDate.Value);
            }
            else if (queryModel.StartDate.HasValue && !queryModel.EndDate.HasValue)
            {
                tempQuery = tempQuery
                    .Where(x => x.DateCreated >= queryModel.StartDate.Value);
            }
            else if (!queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
            {
                tempQuery = tempQuery
                    .Where(x => x.DateCreated <= queryModel.EndDate.Value);
            }

            tempQuery = EntityFilter(tempQuery, queryModel);

            tempQuery = tempQuery.OrderByDescending(x => x.DateCreated);

            List<ApplicationRole> tolist = await tempQuery.ToListAsync();

            var mappedList = _mapper.Map<List<ApplicationRoleViewModel>>(tolist);

            return new Response<List<ApplicationRoleViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }

        public async Task<Response<ApplicationRoleViewModel>> GetItemAsync(Guid id)
        {
            var existingItem = await GetAll()
                    .Where(x => x.Id == id).FirstOrDefaultAsync();

            var mappedList = _mapper.Map<ApplicationRoleViewModel>(existingItem);

            return new Response<ApplicationRoleViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }

        public async Task<Response<List<ApplicationRoleViewModel>>> GetRolesAsync()
        {
            var existingItems = await GetAll().ToListAsync();

            var mappedList = _mapper.Map<List<ApplicationRoleViewModel>>(existingItems);

            return new Response<List<ApplicationRoleViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }

        private IQueryable<ApplicationRole> EntityFilter(IQueryable<ApplicationRole> query, ExportQueryModel model)
        {
            if (model != null && query != null && !string.IsNullOrEmpty(model.Filter) && !string.IsNullOrEmpty(model.Keyword))
            {
                var keyWord = model.Keyword.Trim().ToLower();

                switch (model.Filter.ToLower())
                {
                    case "id":
                        {
                            query = query.Where(x => x.Id.ToString().ToLower() == keyWord);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return query;
        }
    }
}

