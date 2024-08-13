using MTMiddleware.Core.Auth;
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Shared.Models;
using MTMiddleware.Shared.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using MTMiddleware.Core.Helpers.Duo;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using UtilityLibrary.Extensions;
using MTMiddleware.Shared.Pagination;
using System.Reflection;
//using Elastic.Apm.Api;
using Microsoft.AspNetCore.Http;
using Elastic.Apm.Api;
using System.Web.Mvc;
using MTMiddleware.Core.ExternalServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.OpenApi.Extensions;
//using System.Web.Mvc;
//using System.Web.Mvc;

namespace MTMiddleware.Core.Services
{
    public class DashboardService : BaseService<ApplicationUser, Guid>, IDashboardService
    {
        private readonly ILogger<DashboardService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        private readonly IMailerService _mailerService;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _config;
        private readonly IApiCaller _apiCaller;
        private readonly AppDbContext _dBcontext;


        private readonly IExternalAPIServices _externalApiServices;

        private readonly IRepository<CustomerDetails, string> _customerDetailsRepo;
        private readonly IRepository<CustomersChannelTransKey, string> _customerChannelTransKeyRepo;
        private readonly IRepository<CustomerTransactions, string> _customerTransactionRepo;
        private readonly IRepository<CustomerAccounts, string> _customerAccountsRepo;

        private readonly IRepository<ApplicationRole, Guid> _roleRepository;

        public DashboardService(ILogger<DashboardService> logger, IUnitOfWork unitOfWork, IMapper mapper,
            UserManager<ApplicationUser> userManager, IJwtService jwtService, IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> appSettings, IMailerService mailerService,
            IDataProtectionProvider provider, IConfiguration config, IExternalAPIServices externalApiServices,
            AppDbContext dBcontext,
        IApiCaller apiCaller) : base(unitOfWork)
        {

            _httpContextAccessor = httpContextAccessor;
            _externalApiServices = externalApiServices;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _customerDetailsRepo = _unitOfWork.GetRepository<CustomerDetails, string>();
            _customerChannelTransKeyRepo = _unitOfWork.GetRepository<CustomersChannelTransKey, string>();
            _customerTransactionRepo = _unitOfWork.GetRepository<CustomerTransactions, string>();
            _customerAccountsRepo = _unitOfWork.GetRepository<CustomerAccounts, string>();
            _logger = logger; _config = config;
            _jwtService = jwtService;
            _mailerService = mailerService;
            _protector = provider.CreateProtector("MTMiddleware.Core.Services.UserService");
            _apiCaller = apiCaller;
            _dBcontext = dBcontext;
            _roleRepository = _unitOfWork.GetRepository<ApplicationRole, Guid>();

        }


        public async Task<Response<CustomerDashboardViewModel>> GetACustomersDashboardDetails(string accountNo)
        {
            var data = new CustomerDashboardViewModel();

            //       public decimal SwiftTotal { get; set; }
            //public decimal SwiftPending { get; set; }
            //public decimal SwiftSuccessful { get; set; }
            //public decimal SwiftDailyLimit { get; set; }

            //public decimal RTGSTotal { get; set; }
            //public decimal RTGSPending { get; set; }
            //public decimal RTGSSuccessful { get; set; }
            //public decimal RTGSDailyLimit { get; set; }

            var getDetails = _dBcontext.CustomerAccounts.Where(x => x.accountNo == accountNo).FirstOrDefault();
            if (getDetails is null)
            {
                return new Response<CustomerDashboardViewModel>() { Code = ResponseEnum.AccountNoDoesNotExist.ResponseCode(), Description = ResponseEnum.AccountNoDoesNotExist.Description() };

            }

            var customerDetails = _customerDetailsRepo.GetItems(x => x.CifId == getDetails.CifId).FirstOrDefault();

            if (customerDetails is null)
            {
                return new Response<CustomerDashboardViewModel>() { Code = ResponseEnum.CustomerRecordDoesNotExist.ResponseCode(), Description = ResponseEnum.CustomerRecordDoesNotExist.Description() };

            }



            var swiftTrans = _customerTransactionRepo.GetItems(
                x => x.TransactionType.ToLower() == TransactionType.SWIFT.GetDisplayName().ToLower()
            && x.SourceAccount.ToLower() == accountNo.ToLower()).Count();

            var swiftTransPending = _customerTransactionRepo.GetItems(
          x => x.TransactionType.ToLower() == TransactionType.SWIFT.GetDisplayName().ToLower()
      && x.SourceAccount.ToLower() == accountNo.ToLower()
      && x.TransactionStatus == TransactionStatusEnum.Pending.GetDisplayName()
      ).Count();

            var swiftTransSuccessful = _customerTransactionRepo.GetItems(
        x => x.TransactionType.ToLower() == TransactionType.SWIFT.GetDisplayName().ToLower()
    && x.SourceAccount.ToLower() == accountNo.ToLower()
      && x.TransactionStatus == TransactionStatusEnum.Successful.GetDisplayName()
    ).Count();


            var RTGSTrans = _customerTransactionRepo.GetItems(
                x => x.TransactionType.ToLower() == TransactionType.RTGS.GetDisplayName().ToLower()
            && x.SourceAccount.ToLower() == accountNo.ToLower()).Count();

            var RTGSTransPending = _customerTransactionRepo.GetItems(
               x => x.TransactionType.ToLower() == TransactionType.RTGS.GetDisplayName().ToLower()
           && x.SourceAccount.ToLower() == accountNo.ToLower()
      && x.TransactionStatus == TransactionStatusEnum.Pending.GetDisplayName()
           ).Count();

            var RTGSTransSuccessful = _customerTransactionRepo.GetItems(
               x => x.TransactionType.ToLower() == TransactionType.RTGS.GetDisplayName().ToLower()
           && x.SourceAccount.ToLower() == accountNo.ToLower()
      && x.TransactionStatus == TransactionStatusEnum.Successful.GetDisplayName()
           ).Count();


            data.SwiftTotal = swiftTrans;
            data.SwiftPending = swiftTransPending;
            data.SwiftSuccessful = swiftTransSuccessful;
            data.SwiftDailyLimit = customerDetails.SwiftDailyTransactionLimit;


            data.RTGSTotal = RTGSTrans;
            data.RTGSPending = RTGSTransPending;
            data.RTGSSuccessful = RTGSTransSuccessful;
            data.RTGSDailyLimit = customerDetails.RTGSDailyTransactionLimit; ;



            return new Response<CustomerDashboardViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = data
            };
        }
        public async Task<Response<AdminDashboardViewModel>> GetAdminDashboardDetails()
        {

            var resp = new AdminDashboardViewModel();
            //get TotalCustomers

            var customers = _customerDetailsRepo.GetAll();

            var allPending = customers
               .Where(x => x.ApprovalStatus.ToLower() == CustomerApprovalStatusEnum.Pending.GetDisplayName().ToLower());

            resp.TotalCustomers = await customers.CountAsync();
            resp.TotalPendingCustomers = await allPending.CountAsync();

            //all Transactions

            var swiftTrans = await _customerTransactionRepo.GetAll().Where(x => x.TransactionType.ToLower() == TransactionType.SWIFT.GetDisplayName().ToLower()).CountAsync();
            var RTGSTrans = await _customerTransactionRepo.GetAll().Where(x => x.TransactionType.ToLower() == TransactionType.RTGS.GetDisplayName().ToLower()).CountAsync();

            resp.TotatSWIFTTransactions = swiftTrans;
            resp.TotalRTGSTransactions = RTGSTrans;


            return new Response<AdminDashboardViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = resp
            };
        }
    }
}

