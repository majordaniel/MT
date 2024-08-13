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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using System.Web.Mvc;
//using System.Web.Mvc;

namespace MTMiddleware.Core.Services
{
    public class CustomerService : BaseService<ApplicationUser, Guid>, ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
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

        public CustomerService(ILogger<CustomerService> logger, IUnitOfWork unitOfWork, IMapper mapper,
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


        public async Task<Response<ApplicationUserViewModel>> RegisterCustomerAsync(RegisterCustomerRequestViewModel model, DateTime currentDateTime)
        {
            try
            {


                if (model is null || string.IsNullOrEmpty(model.Email))
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                var roleResult = await _roleRepository.GetAll()
                       .Where(x => x.Name.ToLower() == RoleEnum.Customer.Description().ToLower()).FirstOrDefaultAsync();

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountAlreadyExists.ResponseCode(), Description = ResponseEnum.AccountAlreadyExists.Description() };
                }

                var role = string.Empty;

                if (roleResult == null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.RoleAssignedToUserNotFound.ResponseCode(), Description = ResponseEnum.RoleAssignedToUserNotFound.Description() };
                }


                //get the customerId of the user using the account number

                var customerDetails = _externalApiServices.GetDetailsofAnAccount(model.AccountNo).Result;

                if (customerDetails.data is null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountNoDoesNotExist.ResponseCode(), Description = ResponseEnum.AccountNoDoesNotExist.Description() };

                }
                //check if the customer CIF has been saved before now into the customerDetails table

                var ifCifExist = _customerAccountsRepo.GetItems(x => x.CifId == customerDetails.data.customerId).Any();
                if (ifCifExist)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountAlreadyExists.ResponseCode(), Description = ResponseEnum.AccountAlreadyExists.Description() };

                }


                role = roleResult.Id.ToString();


                var user = _mapper.Map<ApplicationUser>(model);

                user.RoleId = role;

                var resetToken = await generateResetTokenAsync();

                user.UserName = model.Email;
                user.DisplayName = customerDetails.data.accountName != null ? customerDetails.data.accountName : string.Empty;
                user.IsActive = true;
                user.PhoneNumber = customerDetails.data.phoneNumber;
                user.FirstName = customerDetails.data.accountName != null ? customerDetails.data.accountName : string.Empty;
                user.LastName = model.AccountNo;
                user.IsRootAdmin = false;
                user.DateCreated = currentDateTime;
                user.EmailVerificationToken = resetToken;

                var result = await _userManager.CreateAsync(user, model.Password);


                if (result == null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.UserInvitationFailed.ResponseCode(), Description = ResponseEnum.UserInvitationFailed.Description() };
                }

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);

                    var firstError = errors.FirstOrDefault();

                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = string.IsNullOrEmpty(firstError) ? ResponseEnum.UserInvitationFailed.Description() : firstError };
                }

                var mappedData = _mapper.Map<ApplicationUserViewModel>(user);
                mappedData.Role = roleResult.Name;
                //Insert the details of the customer to the customer table 
                var newcustomer = new CustomerDetails
                {
                    CustomerName = customerDetails.data.accountName != null ? customerDetails.data.accountName : string.Empty,
                    CifId = customerDetails.data.customerId,
                    PhoneNumber = customerDetails.data.phoneNumber,
                    Email = model.Email,
                    IsActive = false,
                    DateCreated = DateTime.Now,
                    DateLastUpdated = DateTime.Now,
                    ApprovalStatus = CustomerApprovalStatusEnum.Pending.GetDisplayName().ToUpper()

                };
                newcustomer.ApplicationUser = user;

                var saveCustomer = await _customerDetailsRepo.AddAsync(newcustomer);

                //save the lists of customers accounts
                //use the customerid to pull all the customer other accounts

                var allCustomerAccounts = await _externalApiServices.GetListOfAccountByCIF(customerDetails.data.customerId);

                if (allCustomerAccounts.data.Count > 0)
                {
                    var AllAccts = new List<CustomerAccounts>();

                    foreach (var item in allCustomerAccounts.data)
                    {
                        var toAdd = new CustomerAccounts
                        {
                            currency = item.currency,
                            CifId = item.customerId,
                            accountName = item.accountName,
                            accountNo = item.accountNo,
                            accountType = item.accountType,
                            email = item.email,
                            fullName = item.accountName,
                            phoneNumber = item.phoneNumber,
                            IsActive = true,
                            DateCreated = DateTime.Now,
                            DateLastUpdated = DateTime.Now
                        };
                        AllAccts.Add(toAdd);
                        //var resAcct = _customerAccountsRepo.AddAsync(toAdd).Result;
                    }

                    _customerAccountsRepo.AddRange(AllAccts);

                }

                //Insert the details of the customer subscribed channels and the Keys

                string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");

                if (!string.IsNullOrEmpty(saveCustomer.Entity.Id))
                {
                    var listAllKeys = new List<CustomersChannelTransKey>();

                    foreach (var channelsName in model.ChannelIds)
                    {
                        var TokenStr = $"{channelsName.ToUpper()}:{saveCustomer.Entity.Id}";
                        var newCustomerChannels = new CustomersChannelTransKey
                        {
                            Channel = channelsName.ToUpper(),
                            CustomerDetails = saveCustomer.Entity,
                            TransKey = Utility.Encrypt(TokenStr, cryptoKey),
                            IsActive = true,
                            DateCreated = DateTime.Now,
                            CustomerDetailsId = saveCustomer.Entity.Id,
                            DateLastUpdated = DateTime.Now
                        };

                        listAllKeys.Add(newCustomerChannels);
                        //var saveCutomerChgannel = _customerChannelTransKeyRepo.AddAsync(newCustomerChannels);
                    };
                    _customerChannelTransKeyRepo.AddRange(listAllKeys);


                }

                await _mailerService.SendUserInvitationMailAsync(user);

                return new Response<ApplicationUserViewModel>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = mappedData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }


        private async Task<string> generateResetTokenAsync()
        {
            try
            {
                // token is a cryptographically strong random sequence of values
                var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

                // ensure token is unique by checking against db

                var users = await GetAll()
                    .Where(x => x.EmailVerificationToken == token).ToListAsync();

                if (users != null && users.Count > 0)
                    return await generateResetTokenAsync();

                return token;

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return ResponseEnum.ErrorOccured.Description();

            }
        }

        private async Task<string> generateVerificationTokenAsync()
        {
            try
            {
                // token is a cryptographically strong random sequence of values
                var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

                var users = await GetAll()
                    .Where(x => x.EmailVerificationToken == token).ToListAsync();

                if (users != null && users.Count > 0)
                    return await generateVerificationTokenAsync();

                return token;

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return ResponseEnum.ErrorOccured.Description();

            }
        }

        public async Task<Response<ApplicationUserViewModel>>
            UpdateCustomerApprovalAndLimit(UpdateCustomerApprovalAndLimit model)
        {
            try
            {
                var customer = await _dBcontext.CustomerDetails
                    .Include(x => x.ApplicationUser)
                    .Where(x=>x.Id == model.CustomerId)
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
                }

                var user = await _userManager.FindByIdAsync(customer.ApplicationUser.Id.ToString());

                if (user == null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
                }


                var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Id.ToString() == user.RoleId).FirstOrDefaultAsync();

                if (roleResult != null && roleResult.Name.ToUpper() != RoleEnum.Customer.Description().ToUpper())
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.CustomerRecordDoesNotExist.ResponseCode(), Description = ResponseEnum.CustomerRecordDoesNotExist.Description() };
                }



                user.IsActive = model.IsApproved;
                user.DateLastUpdated = DateTime.Now;
                var result = await _userManager.UpdateAsync(user);

                //update the details of the customer 
           
                customer.IsActive = model.IsApproved;
                customer.RTGSDailyTransactionLimit = model.RTGSDailyTransactionLimit;
                customer.SwiftDailyTransactionLimit = model.SwiftDailyTransactionLimit;
                customer.ApprovalActionReason = model.Comment;
                customer.ApprovalActionDate = DateTime.Now;
                customer.ApprovalStatus = model.IsApproved ? CustomerApprovalStatusEnum.Approved.Description() : CustomerApprovalStatusEnum.Declined.Description();


                var update = _customerDetailsRepo.Update(customer.Id, customer);

                if (result.Succeeded && update > 0)
                {
                    var mappedData = _mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);

                    return new Response<ApplicationUserViewModel>()
                    {
                        Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                        Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                        Data = mappedData
                    };
                }
                else
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountCouldNotBeUpdated.ResponseCode(), Description = ResponseEnum.AccountCouldNotBeUpdated.Description() };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }


        public async Task<Response<PagedList<CustomerDetailsResponseViewModel>>> GetAllCustomersDetails(QueryModel queryModel)
        {

            if (queryModel == null)
            {
                return new Response<PagedList<CustomerDetailsResponseViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
            }

            var customers = _customerDetailsRepo.GetAll().Select(x => new CustomerDetailsResponseViewModel
            {
                Id = x.Id,
                ApprovalStatus = x.ApprovalStatus,
                CustomerName = x.CustomerName,
                RTGSDailyTransactionLimit = x.RTGSDailyTransactionLimit,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                SwiftDailyTransactionLimit = x.SwiftDailyTransactionLimit,
                IsActive = x.IsActive,
                UserId = x.ApplicationUser.Id.ToString(),
                DateCreated = x.DateCreated,
                CifId = x.CifId,
                AccountNosViewModel = _dBcontext.CustomerAccounts.Where(y => y.CifId == x.CifId).Select(x => new AccountNosViewModel
                {
                    AccountNo = x.accountNo,
                    Currency = x.currency
                }).ToList(),
                channelTokens = _dBcontext.CustomersChannelTransKey.Where(y => y.CustomerDetailsId == x.Id).Select(x => new CustomerChannelTransKeyResponseViewModel
                {
                    Channel = x.Channel,
                    TransKey = x.TransKey
                }).ToList()

            }).AsQueryable(); ;



            //search

            if (queryModel != null && customers != null && !string.IsNullOrEmpty(queryModel.Filter) && !string.IsNullOrEmpty(queryModel.Keyword))
            {
                var keyWord = queryModel.Keyword.Trim().ToLower();

                switch (queryModel.Filter.ToLower())
                {
                    case "id":
                        {
                            customers = customers.Where(x => x.Id.ToString().ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    case "customername":
                        {
                            customers = customers.Where(x => x.CustomerName.ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    case "cifid":
                        {
                            customers = customers.Where(x => x.CifId.ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }


            //filter

            //if (queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //       .Where(x => x.PaymentDate >= query.StartDate.Value && x.DateCreated <= query.EndDate.Value);
            //}
            //else if (query.StartDate.HasValue && !query.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //        .Where(x => x.PaymentDate >= query.StartDate.Value);
            //}
            //else if (!query.StartDate.HasValue && query.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //        .Where(x => x.PaymentDate <= query.EndDate.Value);
            //}


            //return
            customers = customers?.OrderByDescending(x => x.DateCreated);

            var paginatedData = await customers.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();

            int count = customers.Count();

            var pagedlist = new PagedList<CustomerDetailsResponseViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);


            return new Response<PagedList<CustomerDetailsResponseViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = pagedlist
            };

        }

        public async Task<Response<CustomerDetailsResponseViewModel>> GetACustomersDetails(Guid id)
        {


            var x = _customerDetailsRepo.GetItem(id.ToString());

            if (x == null)
            {
                return new Response<CustomerDetailsResponseViewModel>() { Code = ResponseEnum.CustomerRecordDoesNotExist.ResponseCode(), Description = ResponseEnum.CustomerRecordDoesNotExist.Description() };
            }

            var resp = _dBcontext.CustomerDetails
                 .Include(x => x.ApplicationUser)
                .Where(x => x.Id == id.ToString()).Select(x => new CustomerDetailsResponseViewModel
                {
                    Id = x.Id,
                    ApprovalStatus = x.ApprovalStatus,
                    CustomerName = x.CustomerName,
                    RTGSDailyTransactionLimit = x.RTGSDailyTransactionLimit,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    SwiftDailyTransactionLimit = x.SwiftDailyTransactionLimit,
                    IsActive = x.IsActive,
                    UserId = x.ApplicationUser.Id.ToString(),
                    DateCreated = x.DateCreated,
                    CifId = x.CifId,

                    AccountNosViewModel = _dBcontext.CustomerAccounts.Where(y => y.CifId == x.CifId).Select(x => new AccountNosViewModel
                    {
                        AccountNo = x.accountNo,
                        Currency = x.currency
                    }).ToList(),
                    channelTokens = _dBcontext.CustomersChannelTransKey.Where(y => y.CustomerDetailsId == x.Id).Select(x => new CustomerChannelTransKeyResponseViewModel
                    {
                        Channel = x.Channel,
                        TransKey = x.TransKey
                    }).ToList()

                }).FirstOrDefaultAsync().Result;


            //resp.AccountNosViewModel = _customerAccountsRepo.GetItems(x => x.CifId == resp.CifId).Select(x => new AccountNosViewModel
            //{
            //    AccountNo = x.accountNo,
            //    Currency = x.currency
            //}).ToList();

            //resp.channelTokens = _customerChannelTransKeyRepo.GetItems(x => x.CustomerDetailsId == resp.Id).Select(x => new CustomerChannelTransKeyResponseViewModel
            //{
            //    Channel = x.Channel,
            //    TransKey = x.TransKey
            //}).ToList();


            return new Response<CustomerDetailsResponseViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = resp
            };
        }

        public async Task<Response<PagedList<CustomerDetailsResponseViewModel>>> GetAllPendingCustomersDetails(QueryModel queryModel)
        {

            if (queryModel == null)
            {
                return new Response<PagedList<CustomerDetailsResponseViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
            }

            var customers = _customerDetailsRepo.GetAll()
                .Where(x => x.ApprovalStatus.ToLower() == CustomerApprovalStatusEnum.Pending.GetDisplayName().ToLower())
                .Select(x => new CustomerDetailsResponseViewModel
                {
                    Id = x.Id,
                    ApprovalStatus = x.ApprovalStatus,
                    CustomerName = x.CustomerName,
                    RTGSDailyTransactionLimit = x.RTGSDailyTransactionLimit,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    SwiftDailyTransactionLimit = x.SwiftDailyTransactionLimit,
                    IsActive = x.IsActive,
                    UserId = x.ApplicationUser.Id.ToString(),
                    DateCreated = x.DateCreated,
                    CifId = x.CifId,
                    AccountNosViewModel = _dBcontext.CustomerAccounts.Where(y => y.CifId == x.CifId).Select(x => new AccountNosViewModel
                    {
                        AccountNo = x.accountNo,
                        Currency = x.currency
                    }).ToList(),
                    channelTokens = _dBcontext.CustomersChannelTransKey.Where(y => y.CustomerDetailsId == x.Id).Select(x => new CustomerChannelTransKeyResponseViewModel
                    {
                        Channel = x.Channel,
                        TransKey = x.TransKey
                    }).ToList()

                }).AsQueryable(); ;



            //search

            if (queryModel != null && customers != null && !string.IsNullOrEmpty(queryModel.Filter) && !string.IsNullOrEmpty(queryModel.Keyword))
            {
                var keyWord = queryModel.Keyword.Trim().ToLower();

                switch (queryModel.Filter.ToLower())
                {
                    case "id":
                        {
                            customers = customers.Where(x => x.Id.ToString().ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    case "customername":
                        {
                            customers = customers.Where(x => x.CustomerName.ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    case "cifid":
                        {
                            customers = customers.Where(x => x.CifId.ToLower().Contains(keyWord.ToLower()));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }


            //filter

            //if (queryModel.StartDate.HasValue && queryModel.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //       .Where(x => x.PaymentDate >= query.StartDate.Value && x.DateCreated <= query.EndDate.Value);
            //}
            //else if (query.StartDate.HasValue && !query.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //        .Where(x => x.PaymentDate >= query.StartDate.Value);
            //}
            //else if (!query.StartDate.HasValue && query.EndDate.HasValue)
            //{
            //    tempQuery = tempQuery
            //        .Where(x => x.PaymentDate <= query.EndDate.Value);
            //}


            //return
            customers = customers?.OrderByDescending(x => x.DateCreated);

            var paginatedData = await customers.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();

            int count = customers.Count();

            var pagedlist = new PagedList<CustomerDetailsResponseViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);


            return new Response<PagedList<CustomerDetailsResponseViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = pagedlist
            };

        }

        public async Task<Response<bool>> ChangeCustomerStatus(ChangeCustomerStatusViewModel model)
        {
            try
            {
                var customer = await _dBcontext.CustomerDetails
                .Include(x => x.ApplicationUser)
                    .Where(x => x.Id == model.customerId)
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
                }

                var user = await _userManager.FindByIdAsync(customer.ApplicationUser.Id.ToString());

                if (user == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
                }


                var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Id.ToString() == user.RoleId).FirstOrDefaultAsync();

                if (roleResult != null && roleResult.Name.ToUpper() != RoleEnum.Customer.Description().ToUpper())
                {
                    return new Response<bool>() { Code = ResponseEnum.CustomerRecordDoesNotExist.ResponseCode(), Description = ResponseEnum.CustomerRecordDoesNotExist.Description() };
                }



                user.IsActive = model.Enable;
                user.DateLastUpdated = DateTime.Now;
                var result = await _userManager.UpdateAsync(user);

                //update the details of the customer 

                customer.IsActive = model.Enable;
                customer.DateLastUpdated = DateTime.Now;
                customer.ApprovalStatus = model.Enable ? CustomerApprovalStatusEnum.Approved.Description() : CustomerApprovalStatusEnum.Disabled.Description();


                var update = _customerDetailsRepo.Update(customer.Id, customer);

                if (result.Succeeded && update > 0)
                {
                    var mappedData = _mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);

                    return new Response<bool>()
                    {
                        Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                        Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                        Data = true
                    };
                }
                else
                {
                    return new Response<bool>() { Code = ResponseEnum.AccountCouldNotBeUpdated.ResponseCode(), Description = ResponseEnum.AccountCouldNotBeUpdated.Description() };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }

        }
    }
}

