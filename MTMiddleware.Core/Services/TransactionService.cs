using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Core.ExternalServices;
using MTMiddleware.Shared.Models;
using MTMiddleware.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MTMiddleware.Core.Auth;
using Microsoft.Extensions.Configuration;

namespace MTMiddleware.Core.Services
{
    public class TransactionService : BaseService<CustomerTransactions, string>, ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;
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

        public TransactionService(ILogger<TransactionService> logger, IUnitOfWork unitOfWork, IMapper mapper,
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



        public async Task<Response<CreateTransactionResponse>> CreateTransaction(CreateTransactionRequest model)
        {
            try
            {
                string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");

                var extractedTransKey = _httpContextAccessor.HttpContext.Request.Headers["TRANSKEY"];
                var authHeader = Utility.Decrypt(extractedTransKey.ToString(), cryptoKey);
                var authHeaderParts = authHeader.Split(':');
                var channel = authHeaderParts[0];
                var customerId = authHeaderParts[1];

                var getTransKeyDetails = await _customerChannelTransKeyRepo.GetAll()
                    .Where(x => x.TransKey == extractedTransKey).FirstOrDefaultAsync();

                //you can check if the customerId has a CIFId that is tied to the Transaction Sender account no

                //QUESTION: how do we determing that the transaction is swift or RTGS

                //call the External API here, if the response is successful then log the transaction on the table


                //log the transaction request
                string TransactionId = string.Empty;
                var TransactionDetail = _customerTransactionRepo.AddAsync(new CustomerTransactions
                {
                    Channel = channel,
                    CustomerDetailId = customerId,
                    DateCreated = DateTime.Now,
                    DateLastUpdated = DateTime.Now,
                    TransactionDate = DateTime.Now,
                    ApprovalStatus = TransactionStatusEnum.Pending.GetDisplayName(),
                    TransactionStatus = TransactionStatusEnum.Pending.GetDisplayName(),
                    Amount = model.Amount,
                    SourceAccount = model.SourceAccount,
                    DestinationAccount = model.DestinationAccount,
                    DestinationBank = model.DestinationBank,
                    SourceBank = model.SourceBank

                }).Result;


                return new Response<CreateTransactionResponse>
                {
                    Code = "00",
                    Description = $"Transaction Request sent successfully with TransactionId {TransactionDetail.Entity.Id}",
                    Data = new CreateTransactionResponse
                    {
                        TransactionId = TransactionDetail.Entity.Id,
                        Channel = getTransKeyDetails.Channel
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<CreateTransactionResponse>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<PagedList<CustomerTransactionResponseViewModel>>> 
            GetAllCustomerSwiftTransaction(DateRangeQueryModel queryModel, string CustomerId)
        {
            try
            {
                if (queryModel == null)
                {
                    return new Response<PagedList<CustomerTransactionResponseViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
                }

                //var trans = _customerTransactionRepo.GetItems
                var trans = _dBcontext.CustomerTransactions.Where
                    (x=>x.CustomerDetailId== CustomerId.ToString())
                    .Where(x => x.TransactionType.ToLower() == TransactionType.SWIFT.GetDisplayName().ToLower()
                    && x.CustomerDetailId ==CustomerId)
                    .Select(x => new CustomerTransactionResponseViewModel
                {
                    TransactionDate = x.TransactionDate,
                    Channel = x.Channel


                }).AsQueryable(); ;


                //filter

                if (queryModel != null && trans != null && !string.IsNullOrEmpty(queryModel.Filter) && !string.IsNullOrEmpty(queryModel.Keyword))
                {
                    var keyWord = queryModel.Keyword.Trim().ToLower();

                    switch (queryModel.Filter.ToLower())
                    {
                        case "channel":
                            {
                                trans = trans.Where(x => x.Channel.ToLower().ToString().Contains(keyWord.ToLower()));
                                break;
                            }
                        case "status":
                            {
                                trans = trans.Where(x => x.TransactionStatus.ToLower().Contains(keyWord.ToLower()));
                                break;
                            }

                        case "ref":
                            {
                                trans = trans.Where(x => x.Reference.ToLower().Contains(keyWord.ToLower()));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }


                //search //startdate and enddate

                if (queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
                {
                    trans = trans
                       .Where(x => x.TransactionDate >= queryModel.StartDate.Value && x.TransactionDate <= queryModel.EndDate.Value);
                }
                else if (queryModel.StartDate.HasValue && !queryModel.EndDate.HasValue)
                {
                    trans = trans
                        .Where(x => x.TransactionDate >= queryModel.StartDate.Value);
                }
                else if (!queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
                {
                    trans = trans
                        .Where(x => x.TransactionDate <= queryModel.EndDate.Value);
                }



                trans = trans?.OrderByDescending(x => x.TransactionDate);


                //query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                //trans=  trans.Skip((queryModel.PageNumber - 1) * queryModel.PageSize).Take(queryModel.PageSize);

                var paginatedData = await trans.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();



                int count = trans.Count();

                var pagedlist = new PagedList<CustomerTransactionResponseViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);


                return new Response<PagedList<CustomerTransactionResponseViewModel>>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    //Data = trans.ToList(),
                    Data = pagedlist,
                };




            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<PagedList<CustomerTransactionResponseViewModel>>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<PagedList<CustomerTransactionResponseViewModel>>> 
            GetAllCustomerRTGSTransaction(DateRangeQueryModel queryModel, string CustomerId)
        {

            try
            {

                if (queryModel == null)
                {
                    return new Response<PagedList<CustomerTransactionResponseViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
                }


                var trans = _dBcontext.CustomerTransactions.Where
                 (x => x.CustomerDetailId == CustomerId.ToString())
                 .Where(x => x.TransactionType.ToLower() == TransactionType.RTGS.GetDisplayName().ToLower()
                 && x.CustomerDetailId == CustomerId)
                 .Select(x => new CustomerTransactionResponseViewModel
                 {
                     TransactionDate = x.TransactionDate,
                     Channel = x.Channel


                 }).AsQueryable(); ;
              
                //filter

                if (queryModel != null && trans != null && !string.IsNullOrEmpty(queryModel.Filter) && !string.IsNullOrEmpty(queryModel.Keyword))
                {
                    var keyWord = queryModel.Keyword.Trim().ToLower();

                    switch (queryModel.Filter.ToLower())
                    {
                        case "channel":
                            {
                                trans = trans.Where(x => x.Channel.ToLower().ToString().Contains(keyWord.ToLower()));
                                break;
                            }
                        case "status":
                            {
                                trans = trans.Where(x => x.TransactionStatus.ToLower().Contains(keyWord.ToLower()));
                                break;
                            }
                        case "ref":
                            {
                                trans = trans.Where(x => x.Reference.ToLower().Contains(keyWord.ToLower()));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }


                //search //startdate and enddate

                if (queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
                {
                    trans = trans
                       .Where(x => x.TransactionDate >= queryModel.StartDate.Value && x.TransactionDate <= queryModel.EndDate.Value);
                }
                else if (queryModel.StartDate.HasValue && !queryModel.EndDate.HasValue)
                {
                    trans = trans
                        .Where(x => x.TransactionDate >= queryModel.StartDate.Value);
                }
                else if (!queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
                {
                    trans = trans
                        .Where(x => x.TransactionDate <= queryModel.EndDate.Value);
                }



                trans = trans?.OrderByDescending(x => x.TransactionDate);

                var paginatedData = await trans.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();



                int count = trans.Count();

                var pagedlist = new PagedList<CustomerTransactionResponseViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);


                return new Response<PagedList<CustomerTransactionResponseViewModel>>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = pagedlist
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<PagedList<CustomerTransactionResponseViewModel>>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

    }
}
