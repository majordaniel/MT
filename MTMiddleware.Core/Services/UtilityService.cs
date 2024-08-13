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
    public class UtilityService : BaseService<ApplicationUser, Guid>, IUtilityService
    {
        private readonly ILogger<UtilityService> _logger;
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

        public UtilityService(ILogger<UtilityService> logger, IUnitOfWork unitOfWork, IMapper mapper,
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


        public async Task<Response<string>> EncryptAsync(string TextToEncrypt)
        {


            var result = string.Empty;
            string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");
            result = Utility.Encrypt(TextToEncrypt, cryptoKey);

            return new Response<string>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = result
            };

        }

        public async Task<Response<string>> DecryptAsync(string EncryptedText)
        {
            var result = string.Empty;
            string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");
            result = Utility.Decrypt(EncryptedText, cryptoKey);

            return new Response<string>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = result
            };
        }
    }

}