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
using System.Net.Http;
using System.Web.Helpers;
//using System.Web.Mvc;
//using System.Web.Mvc;

namespace MTMiddleware.Core.Services
{
    public class UserService : BaseService<ApplicationUser, Guid>, IUserService
    {
        private readonly ILogger<UserService> _logger;
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

        public UserService(ILogger<UserService> logger, IUnitOfWork unitOfWork, IMapper mapper,
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


        public async Task<Response<ApplicationUserViewModel>> InviteAsync(InviteUserViewModel model, DateTime currentDateTime)
        {
            try
            {
                if (model is null || string.IsNullOrEmpty(model.Email))
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountAlreadyExists.ResponseCode(), Description = ResponseEnum.AccountAlreadyExists.Description() };
                }

                string role = String.Empty;

                if (model.RoleId != Guid.Empty)
                {
                    var roleResult = await _roleRepository.GetAll()
                        .Where(x => x.Id == model.RoleId).FirstOrDefaultAsync();

                    if (roleResult == null)
                    {
                        return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.RoleAssignedToUserNotFound.ResponseCode(), Description = ResponseEnum.RoleAssignedToUserNotFound.Description() };
                    }

                    role = roleResult.Name;
                }

                var user = _mapper.Map<ApplicationUser>(model);

                var resetToken = await generateResetTokenAsync();

                var getUserProfileResponse = await GetUserProfileAsync(model.Email);


                if (getUserProfileResponse.Data is null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.NoADRecordsFound.ResponseCode(), Description = ResponseEnum.NoADRecordsFound.Description() };
                }


                ADUserProfileViewModel? userProfile = null;
                if (getUserProfileResponse != null && getUserProfileResponse.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode() && getUserProfileResponse.Data != null)
                {
                    userProfile = getUserProfileResponse.Data;
                }

                user.UserName = model.Email;
                user.DisplayName = userProfile != null ? userProfile.DisplayName : string.Empty;
                user.FirstName = userProfile != null ? userProfile.DisplayName : string.Empty;
                user.UserName = userProfile != null ? userProfile.UserName : string.Empty;
                user.IsActive = true;
                user.IsRootAdmin = false;
                user.DateCreated = currentDateTime;
                user.EmailVerificationToken = resetToken;




                var randomPassword = Utility.CreateRandomPasswordWithRandomLength();
                var result = await _userManager.CreateAsync(user, randomPassword);



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
                mappedData.Role = role;
                await _mailerService.SendUserInvitationMailAsync(user);

                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(), Description = ResponseEnum.OperationCompletedSuccesfully.Description(), Data = mappedData };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<SignInViewModel>> SignInAsync(AuthCredentialViewModel model, DateTime currentTime)
        {
            _logger.LogInformation($"Attempting to signin started ...");

            try
            {

                if (model is null || string.IsNullOrEmpty(model.email))
                {
                    return new Response<SignInViewModel>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                if (_appSettings == null || _appSettings.JwtSettings == null)
                {
                    return new Response<SignInViewModel>() { Code = ResponseEnum.MissingConfigurationData.ResponseCode(), Description = ResponseEnum.MissingConfigurationData.Description() };
                }

                var appSettings = _appSettings;

                var jwtSettings = appSettings.JwtSettings;

                var response = new Response<SignInViewModel>();
                var mappedData = new SignInViewModel();

                string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");

                string decryptedPassword = Utility.Decrypt(model.password, cryptoKey);
                if (string.IsNullOrEmpty(decryptedPassword))
                {
                    return new Response<SignInViewModel>() { Code = ResponseEnum.InvalidUsernameOrPassword.ResponseCode(), Description = ResponseEnum.InvalidUsernameOrPassword.Description() };

                }

                model.password = decryptedPassword;


                //var user = await _userManager.FindByNameAsync(model.email);
                var user = await _userManager.FindByEmailAsync(model.email);


                if (user == null)
                {
                    return new Response<SignInViewModel>() { Code = ResponseEnum.InvalidUsernameOrPassword.ResponseCode(), Description = ResponseEnum.InvalidUsernameOrPassword.Description() };
                }

                _logger.LogInformation($"User {model.email} found in db...");

                if (!user.IsActive)
                {
                    _logger.LogInformation($"{model.email}: account has been disabled");

                    return new Response<SignInViewModel>() { Code = ResponseEnum.UnableToSignIn.ResponseCode(), Description = ResponseEnum.UnableToSignIn.Description() };
                }

                string role = String.Empty;
                //Guid role = Guid.Empty;
                ApplicationRole roleResult = null;
                Guid.TryParse(user.RoleId, out Guid roleId);
                if (!string.IsNullOrEmpty(user.RoleId))
                //if (user.RoleId!=null)
                {
                    roleResult = await _roleRepository.GetAll()
                       .Where(x => x.Id == roleId).FirstOrDefaultAsync();

                    if (roleResult == null)
                    {
                        return new Response<SignInViewModel>() { Code = ResponseEnum.RoleAssignedToUserNotFound.ResponseCode(), Description = ResponseEnum.RoleAssignedToUserNotFound.Description() };
                    }

                    role = roleResult.Name;

                    _logger.LogInformation($"{model.email} is assigned to {role} role...");
                }

                var disableADAuth = false; //TODO: Read this from AppSettings
                //Read from Appsettings

                //var disableADAuthResult = await _businessOptionService.DisableADAuthenticationAsync();
                //if (disableADAuthResult != null)
                //{
                //    disableADAuth = disableADAuthResult.Data;
                //}

                if (disableADAuth)
                {
                    _logger.LogInformation($"AD authentication has been disabled ...");
                }
                else
                {
                    _logger.LogInformation($"AD authentication is active ...");
                }

                if (disableADAuth)
                {
                    _logger.LogInformation($"Attempting to signin for {model.email} ...");

                    Dictionary<string, object> claims = new Dictionary<string, object>();
                    claims.Add(ClaimTypes.Name, user.UserName);
                    claims.Add(ClaimTypes.NameIdentifier, user.Id.ToString());
                    claims.Add(ClaimTypes.Email, user.Email);
                    claims.Add("dspname", user.DisplayName);
                    claims.Add("userId", user.Id.ToString());
                    claims.Add("signindate", currentTime.ToString());

                    string? token = _jwtService.GenerateToken(user.Id.ToString(), claims);
                    if (token == null)
                    {
                        response.Data = null;

                        return response;
                    }

                    string refreshToken = _jwtService.GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.refreshTokenExpiryInDays);
                    var update = await _userManager.UpdateAsync(user);

                    mappedData = _mapper.Map<ApplicationUser, SignInViewModel>(user);
                    if (mappedData == null)
                    {
                        response.Data = null;

                        return response;
                    }

                    mappedData.Role = role;
                    mappedData.AccessToken = token;
                    mappedData.RefreshToken = refreshToken;

                    return new Response<SignInViewModel>()
                    {
                        Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                        Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                        Data = mappedData
                    };
                }

                //Sign In as an Internal Staff

                if (roleResult?.Name.ToUpper() != RoleEnum.Customer.Description().ToUpper())
                {

                    var authenticationResponse = await AuthenticateAsync(model);
                    if (authenticationResponse == null)
                    {
                        return new Response<SignInViewModel>() { Code = ResponseEnum.UnableToSignIn.ResponseCode(), Description = ResponseEnum.UnableToSignIn.Description() };
                    }
                    else if (authenticationResponse.Code != ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                    {
                        return new Response<SignInViewModel>
                        {
                            Code = authenticationResponse.Code,
                            Description = authenticationResponse.Description,
                            Data = null
                        };
                    }
                    else
                    {
                        Dictionary<string, object> claims = new Dictionary<string, object>();
                        claims.Add(ClaimTypes.Name, user.UserName);
                        claims.Add(ClaimTypes.NameIdentifier, user.Id.ToString());
                        claims.Add(ClaimTypes.Email, user.Email);
                        claims.Add("dspname", user.DisplayName);
                        claims.Add("signindate", currentTime.ToString());

                        string? token = _jwtService.GenerateToken(user.Id.ToString(), claims);
                        if (token == null)
                        {
                            response.Data = null;

                            return response;
                        }

                        string refreshToken = _jwtService.GenerateRefreshToken();

                        user.RefreshToken = refreshToken;
                        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.refreshTokenExpiryInDays);
                        var update = await _userManager.UpdateAsync(user);

                        mappedData = _mapper.Map<ApplicationUser, SignInViewModel>(user);
                        if (mappedData == null)
                        {
                            response.Data = null;

                            return response;
                        }

                        mappedData.Role = role;
                        mappedData.AccessToken = token;
                        mappedData.RefreshToken = refreshToken;
                        //irrespective of AD enabled or not get the Duo Service call


                        bool isDuoEnabled = _config.GetValue<bool>("AppSettings:DuoSettings:Enabled");

                        string applicationKey = _config.GetValue<string>("AppSettings:DuoSettings:ApplicationKey");
                        string integrationKey = _config.GetValue<string>("AppSettings:DuoSettings:IntegrationKey");
                        string secretKey = _config.GetValue<string>("AppSettings:DuoSettings:SecretKey");
                        string duoHost = _config.GetValue<string>("AppSettings:DuoSettings:host");

                        string signedRequest = GetSignedRequest(decryptedPassword, applicationKey, integrationKey, secretKey);
                        mappedData.DuoEnabled = isDuoEnabled;
                        mappedData.DuoSignedRequest = signedRequest;
                        mappedData.Host = duoHost;


                    }

                }
                else//Sign In as a customer
                {
                    //sign in
                    var signin = await _userManager.CheckPasswordAsync(user, decryptedPassword);
                    if (!signin)
                    {
                        return new Response<SignInViewModel>() { Code = ResponseEnum.UnableToSignIn.ResponseCode(), Description = ResponseEnum.UnableToSignIn.Description() };
                    }
                    else
                    {
                        Dictionary<string, object> claims = new Dictionary<string, object>();
                        claims.Add(ClaimTypes.Name, user.UserName);
                        claims.Add(ClaimTypes.NameIdentifier, user.Id.ToString());
                        claims.Add(ClaimTypes.Email, user.Email);
                        claims.Add("dspname", user.DisplayName);
                        claims.Add("signindate", currentTime.ToString());

                        string? token = _jwtService.GenerateToken(user.Id.ToString(), claims);
                        if (token == null)
                        {
                            response.Data = null;

                            return response;
                        }

                        string refreshToken = _jwtService.GenerateRefreshToken();

                        user.RefreshToken = refreshToken;
                        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.refreshTokenExpiryInDays);
                        var update = await _userManager.UpdateAsync(user);

                        mappedData = _mapper.Map<ApplicationUser, SignInViewModel>(user);
                        if (mappedData == null)
                        {
                            response.Data = null;

                            return response;
                        }

                        mappedData.Role = role;
                        mappedData.AccessToken = token;
                        mappedData.RefreshToken = refreshToken;
                        //irrespective of AD enabled or not get the Duo Service call


                        bool isDuoEnabled = _config.GetValue<bool>("AppSettings:DuoSettings:Enabled");

                        string applicationKey = _config.GetValue<string>("AppSettings:DuoSettings:ApplicationKey");
                        string integrationKey = _config.GetValue<string>("AppSettings:DuoSettings:IntegrationKey");
                        string secretKey = _config.GetValue<string>("AppSettings:DuoSettings:SecretKey");
                        string duoHost = _config.GetValue<string>("AppSettings:DuoSettings:host");

                        string signedRequest = GetSignedRequest(decryptedPassword, applicationKey, integrationKey, secretKey);
                        mappedData.DuoEnabled = isDuoEnabled;
                        mappedData.DuoSignedRequest = signedRequest;
                        mappedData.Host = duoHost;


                    }

                }

                //Add User Email on the header

                //// Add email to header context
                //httpContext.Request.Headers["X-User-Email"] = email;

                //addUserHeaderContext(role, user.Email, HttpContext)
                // context.HttpContext.Request.Headers["Authorization"];


                _httpContextAccessor.HttpContext.Request.Headers.Add("LoggedinUserRole",role);
                _httpContextAccessor.HttpContext.Request.Headers.Add("LoggedinUserEmail",user.Email);


                return new Response<SignInViewModel>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = mappedData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<SignInViewModel>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<bool>> RequestPasswordResetAsync(EmailViewModel model, DateTime currentDateTime)
        {
            try
            {
                if (model is null)
                {
                    return new Response<bool>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                if (string.IsNullOrEmpty(model.Email))
                {
                    return new Response<bool>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var tokenQueryModel = new PasswordResetQueryModel { Email = user.Email, Token = code };
                var tokenQueryModelString = JsonConvert.SerializeObject(tokenQueryModel);
                code = _protector.Protect(tokenQueryModelString);

                await _mailerService.SendRequestPasswordResetMailAsync(user, code);

                return new Response<bool>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = true
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordViewModel model, DateTime currentDateTime)
        {

            var response = new Response<bool>();
            try
            {
                if (model is null)
                {
                    return new Response<bool>() { Code = ResponseEnum.RequiredDataNotProvided.ResponseCode(), Description = ResponseEnum.RequiredDataNotProvided.Description() };
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    return new Response<bool>() { Code = ResponseEnum.PasswordMismatch.ResponseCode(), Description = ResponseEnum.PasswordMismatch.Description() };
                }

                var bareSecurityToken = string.Empty;

                try
                {
                    bareSecurityToken = _protector.Unprotect(model.Token);
                }
                catch (Exception)
                {
                    return new Response<bool>() { Code = ResponseEnum.InvalidSecurityToken.ResponseCode(), Description = ResponseEnum.InvalidSecurityToken.Description() };
                }

                var passwordResetModel = JsonConvert.DeserializeObject<PasswordResetQueryModel>(bareSecurityToken);
                if (passwordResetModel == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.InvalidSecurityToken.ResponseCode(), Description = ResponseEnum.InvalidSecurityToken.Description() };
                }

                var user = await _userManager.FindByEmailAsync(passwordResetModel.Email);
                if (user == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
                }

                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                }

                var res = await _userManager.ResetPasswordAsync(user, passwordResetModel.Token, model.NewPassword);
                if (!res.Succeeded)
                {
                    if (res.Errors.Count() > 0)
                    {
                        var errorMsg = res.Errors.FirstOrDefault();

                        if (errorMsg != null)
                        {
                            return new Response<bool>() { Code = ResponseEnum.PasswordResetFailed.ResponseCode(), Description = ResponseEnum.PasswordResetFailed.Description() + $": {errorMsg.Description}" };
                        }
                        else
                        {
                            return new Response<bool>() { Code = ResponseEnum.PasswordResetFailed.ResponseCode(), Description = ResponseEnum.PasswordResetFailed.Description() };
                        }
                    }
                }

                await _userManager.UpdateAsync(user);

                await _mailerService.SendSuccessfulPasswordResetMailAsync(user);

                return new Response<bool>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = true
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid id, DateTime currentDateTime)
        {
            var response = new Response<bool>();
            try
            {
                var userId = id.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var msg = result.Errors.Select(x => x.Description).FirstOrDefault();
                    return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = string.IsNullOrEmpty(msg) ? ResponseEnum.AccountNotFound.Description() : msg };
                }

                response.Data = true;

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

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

      
        public async Task<Response<ApplicationUserViewModel>> UpdateUserAsync(Guid id, UpdateUserViewModel model, DateTime currentDateTime)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
                }

                if (!string.IsNullOrEmpty(model.FirstName))
                {
                    user.FirstName = model.FirstName;
                }

                if (!string.IsNullOrEmpty(model.LastName))
                {
                    user.LastName = model.LastName;
                }

                user.DisplayName = $"{user.FirstName} {user.LastName}";

                if (!string.IsNullOrEmpty(model.RoleId))
                {
                    user.RoleId = model.RoleId;
                    //user.RoleId = Guid.Parse( model.RoleId);
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
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

        public async Task<Response<ApplicationUserViewModel>> GetUserAsync(Guid id)
        {
            var existingItem = await GetAll()
                    .Where(x => x.Id == id).FirstOrDefaultAsync();

            var mappedList = _mapper.Map<ApplicationUserViewModel>(existingItem);

            return new Response<ApplicationUserViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }

        public async Task<Response<ApplicationUserViewModel>> EnableAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
            }

            if (!user.IsActive)
            {
                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
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
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountCouldNotBeEnabled.ResponseCode(), Description = ResponseEnum.AccountCouldNotBeEnabled.Description() };
                }
            }

            var mappedData2 = _mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);

            return new Response<ApplicationUserViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedData2
            };
        }

        public async Task<Response<ApplicationUserViewModel>> DisableAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
            }

            if (user.IsActive)
            {
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
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
                    return new Response<ApplicationUserViewModel>() { Code = ResponseEnum.AccountCouldNotBeDisabled.ResponseCode(), Description = ResponseEnum.AccountCouldNotBeDisabled.Description() };
                }
            }

            var mappedData2 = _mapper.Map<ApplicationUser, ApplicationUserViewModel>(user);

            return new Response<ApplicationUserViewModel>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedData2
            };
        }

        public async Task<Response<List<ApplicationUserViewModel>>> GetUsersAsync()
        {
            var existingItems = await GetAll()
                    .Where(x => x.IsActive).ToListAsync();

            var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            return new Response<List<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }
        public async Task<Response<List<ApplicationUserViewModel>>> GetApproversUsersAsync()
        {
            var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Name.ToLower() == RoleEnum.Approver.Description().ToLower()).FirstOrDefaultAsync();



            var existingItems = await GetAll()
                   //.Include(x => x.ApplicationRole)

                   //.Where(x => x.IsActive).Where(x => x.ApplicationRole.Id == roleResult.Id)
                   .Where(x => x.IsActive).Where(x => x.RoleId.ToLower() == roleResult.Id.ToString())
                   //.Select(x => new ApplicationUserViewModel
                   //{

                   //})
                   .ToListAsync();

            var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            return new Response<List<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }
        public async Task<Response<List<ApplicationUserViewModel>>> GetSuperAdminUsersAsync()
        {
            var roleResult = await _roleRepository.GetAll()
                   .Where(x => x.Name.ToLower() == RoleEnum.SuperAdmin.Description().ToLower()).FirstOrDefaultAsync();



            var existingItems = await GetAll()
                   //.Include(x => x.ApplicationRole)

                   //.Where(x => x.IsActive).Where(x => x.ApplicationRole.Id == roleResult.Id)
                   .Where(x => x.IsActive).Where(x => x.RoleId.ToLower() == roleResult.Id.ToString())
                   //.Select(x => new ApplicationUserViewModel
                   //{

                   //})
                   .ToListAsync();
            var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            return new Response<List<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }
        public async Task<Response<List<ApplicationUserViewModel>>> GetCustomerUsersAsync()
        {
            var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Name.ToLower() == RoleEnum.Customer.Description().ToLower()).FirstOrDefaultAsync();



            var existingItems = await GetAll()
                   .Include(x => x.CustomerDetails)
                   //.Where(x => x.IsActive).Where(x => x.ApplicationRole.Id == roleResult.Id)
                   .Where(x => x.IsActive).Where(x => x.RoleId.ToLower() == roleResult.Id.ToString())
                   //.Select(x => new ApplicationUserViewModel
                   //{

                   //})
                   .ToListAsync();

            var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            return new Response<List<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = mappedList
            };
        }
        public async Task<Response<PagedList<ApplicationUserViewModel>>> GetCustomerAllUsersAsync(QueryModel queryModel)
        {
            var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Name.ToLower() == RoleEnum.Customer.Description().ToLower()).FirstOrDefaultAsync();



            var existingItems = GetAll()
                   .Include(x => x.CustomerDetails)
                   //.Where(x => x.IsActive).Where(x => x.ApplicationRole.Id == roleResult.Id)
                   .Where(x => x.IsActive).Where(x => x.RoleId.ToLower() == roleResult.Id.ToString())
                   //.Select(x => new ApplicationUserViewModel
                   //{

                   //})
                   .AsQueryable();


            IQueryable<ApplicationUserViewModel> entityQuery = EntitySelect(existingItems);

            entityQuery = EntityFilter(entityQuery, queryModel);

            entityQuery = entityQuery.OrderByDescending(x => x.DateCreated);

            var paginatedData = await entityQuery.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();

            int count = entityQuery.Count();

            var pagedlist = new PagedList<ApplicationUserViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);

            return new Response<PagedList<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = pagedlist
            };


            //var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            //return new Response<List<ApplicationUserViewModel>>()
            //{
            //    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
            //    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
            //    Data = mappedList
            //};
        }
        public async Task<Response<PagedList<ApplicationUserViewModel>>> GetAllcustomerUsersByStatus(QueryModel queryModel, bool isactive)
        {
            var roleResult = await _roleRepository.GetAll()
                  .Where(x => x.Name.ToLower() == RoleEnum.Customer.Description().ToLower()).FirstOrDefaultAsync();



            var existingItems = GetAll()
                   .Include(x => x.CustomerDetails)
                   //.Where(x => x.IsActive).Where(x => x.ApplicationRole.Id == roleResult.Id)
                   .Where(x => x.IsActive == isactive && x.RoleId.ToLower() == roleResult.Id.ToString())
                   //.Select(x => new ApplicationUserViewModel
                   //{

                   //})
                   .AsQueryable();


            IQueryable<ApplicationUserViewModel> entityQuery = EntitySelect(existingItems);

            entityQuery = EntityFilter(entityQuery, queryModel);

            entityQuery = entityQuery.OrderByDescending(x => x.DateCreated);

            var paginatedData = await entityQuery.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();

            int count = entityQuery.Count();

            var pagedlist = new PagedList<ApplicationUserViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);

            return new Response<PagedList<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = pagedlist
            };


            //var mappedList = _mapper.Map<List<ApplicationUserViewModel>>(existingItems);

            //return new Response<List<ApplicationUserViewModel>>()
            //{
            //    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
            //    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
            //    Data = mappedList
            //};
        }
      
        
        private async Task<Response<ADUserProfileViewModel>> GetUserProfileAsync(string email)
        {
            if (_appSettings == null || _appSettings.ADAuthSettings == null || string.IsNullOrEmpty(_appSettings.ADAuthSettings.UserProfileUrl))
            {
                return new Response<ADUserProfileViewModel>() { Code = ResponseEnum.UserProfileUrlNotConfigured.ResponseCode(), Description = ResponseEnum.UserProfileUrlNotConfigured.Description() };
            }

            if (string.IsNullOrEmpty(email))
            {
                return new Response<ADUserProfileViewModel>() { Code = ResponseEnum.ProvideValidEmail.ResponseCode(), Description = ResponseEnum.ProvideValidEmail.Description() };
            }

            string url = _appSettings.ADAuthSettings.UserProfileUrl.ToLower().Replace("{email}", email);
            var headers = new Dictionary<string, string>();

            var apiResponse = await _apiCaller.GetAsync(url, headers);

            if (apiResponse != null && apiResponse.IsSuccessfull && apiResponse.StatusCode == System.Net.HttpStatusCode.OK && apiResponse.Data != null)
            {
                var transformedResponse = JsonConvert.DeserializeObject<Response<ADUserProfileViewModel>>(apiResponse.Data);

                if (transformedResponse == null)
                {
                    return new Response<ADUserProfileViewModel>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
                }
                else if (transformedResponse.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                {
                    return new Response<ADUserProfileViewModel>
                    {
                        Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                        Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                        Data = transformedResponse.Data
                    };
                }
                else
                {
                    return new Response<ADUserProfileViewModel>
                    {
                        Code = transformedResponse.Code,
                        Description = transformedResponse.Description,
                        Data = null
                    };
                }
            }

            return new Response<ADUserProfileViewModel>() { Code = ResponseEnum.UnableToRetrieveUserProfile.ResponseCode(), Description = ResponseEnum.UnableToRetrieveUserProfile.Description() };
        }

        private async Task<Response<bool>> AuthenticateAsync(AuthCredentialViewModel model)
        {
            if (_appSettings == null || _appSettings.ADAuthSettings == null || string.IsNullOrEmpty(_appSettings.ADAuthSettings.SignInUrl))
            {
                return new Response<bool>() { Code = ResponseEnum.SignInUrlNotConfigured.ResponseCode(), Description = ResponseEnum.SignInUrlNotConfigured.Description() };
            }

            if (string.IsNullOrEmpty(model.email))
            {
                return new Response<bool>() { Code = ResponseEnum.ProvideUserName.ResponseCode(), Description = ResponseEnum.ProvideUserName.Description() };
            }

            //string cryptoKey = _appSettings.CryptoKey;

            //string decryptedPassword = Utility.Decrypt(model.password, cryptoKey);
            //if (string.IsNullOrEmpty(decryptedPassword))
            //{
            //    return new Response<bool>() { Code = ResponseEnum.InvalidUsernameOrPassword.ResponseCode(), Description = ResponseEnum.InvalidUsernameOrPassword.Description() };
            //}

            //model.password = decryptedPassword;

            string url = _appSettings.ADAuthSettings.SignInUrl.ToLower();
            var headers = new Dictionary<string, string>();

            var apiResponse = await _apiCaller.PostAsync(url, model, headers);

            _logger.LogInformation($"Authentication POST: {model.email}: Authenticate response:: {JsonConvert.SerializeObject(apiResponse)}");

            if (apiResponse != null && apiResponse.IsSuccessfull && apiResponse.StatusCode == System.Net.HttpStatusCode.OK && apiResponse.Data != null)
            {
                var transformedResponse = JsonConvert.DeserializeObject<Response<bool>>(apiResponse.Data);

                if (transformedResponse == null)
                {
                    return new Response<bool>() { Code = ResponseEnum.UnableToSignIn.ResponseCode(), Description = ResponseEnum.UnableToSignIn.Description() };
                }
                else if (transformedResponse.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode() && transformedResponse.Data)
                {
                    return new Response<bool>
                    {
                        Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                        Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                        Data = transformedResponse.Data
                    };
                }
                else
                {
                    return new Response<bool>
                    {
                        Code = transformedResponse.Code,
                        Description = transformedResponse.Description,
                        Data = false
                    };
                }
            }

            return new Response<bool>() { Code = ResponseEnum.UnableToSignIn.ResponseCode(), Description = ResponseEnum.UnableToSignIn.Description() };
        }

        public async Task<Response<PagedList<ApplicationUserViewModel>>> SearchAsync(QueryModel queryModel, DateTime currentDateTime)
        {
            if (queryModel == null)
            {
                return new Response<PagedList<ApplicationUserViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
            }

            IQueryable<ApplicationUser> tempQuery;

            tempQuery = GetAll();

            IQueryable<ApplicationUserViewModel> entityQuery = EntitySelect(tempQuery);

            entityQuery = EntityFilter(entityQuery, queryModel);

            entityQuery = entityQuery.OrderByDescending(x => x.DateCreated);

            var paginatedData = await entityQuery.Paginate(queryModel.PageNumber, queryModel.PageSize).ToListAsync();

            int count = entityQuery.Count();

            var pagedlist = new PagedList<ApplicationUserViewModel>(paginatedData, queryModel.PageNumber, queryModel.PageSize, count);

            return new Response<PagedList<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = pagedlist
            };
        }

        public async Task<Response<List<ApplicationUserViewModel>>> GetAllUsers(ExportQueryModel queryModel)
        {
            if (queryModel == null)
            {
                return new Response<List<ApplicationUserViewModel>>() { Code = ResponseEnum.ParameterInputNotProvided.ResponseCode(), Description = ResponseEnum.ParameterInputNotProvided.Description() };
            }

            IQueryable<ApplicationUser> tempQuery;

            tempQuery = GetAll();

            IQueryable<ApplicationUserViewModel> entityQuery = EntitySelect(tempQuery);

            entityQuery = EntityFilter(entityQuery, queryModel);

            entityQuery = entityQuery.OrderByDescending(x => x.DateCreated);

            List<ApplicationUserViewModel> list = await entityQuery.ToListAsync();

            return new Response<List<ApplicationUserViewModel>>()
            {
                Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                Data = list
            };
        }

        #region "Private Methods"
        private IQueryable<ApplicationUserViewModel> EntitySelect(IQueryable<ApplicationUser> query)
        {
            return query.Select(x => new ApplicationUserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DisplayName = x.DisplayName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RoleId = x.RoleId,
                IsActive = x.IsActive,
                DateCreated = x.DateCreated
            });
        }

        private IQueryable<ApplicationUserViewModel> EntityFilter(IQueryable<ApplicationUserViewModel> query, QueryModel model)
        {
            if (model != null && query != null && !string.IsNullOrEmpty(model.Filter) && !string.IsNullOrEmpty(model.Keyword))
            {
                var keyWord = model.Keyword.Trim().ToLower();

                switch (model.Filter.ToLower())
                {
                    case "id":
                        {
                            query = query.Where(x => x.Id.ToString().Contains(keyWord));
                            break;
                        }
                    case "username":
                        {
                            query = query.Where(x => x.UserName.Contains(keyWord));
                            break;
                        }
                    case "firstname":
                        {
                            query = query.Where(x => x.FirstName.Contains(keyWord));
                            break;
                        }
                    case "lastname":
                        {
                            query = query.Where(x => x.LastName.Contains(keyWord));
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

        private IQueryable<ApplicationUserViewModel> EntityFilter(IQueryable<ApplicationUserViewModel> query, ExportQueryModel model)
        {
            if (model != null && query != null && !string.IsNullOrEmpty(model.Filter) && !string.IsNullOrEmpty(model.Keyword))
            {
                var keyWord = model.Keyword.Trim().ToLower();

                switch (model.Filter.ToLower())
                {
                    case "id":
                        {
                            query = query.Where(x => x.Id.ToString().Contains(keyWord));
                            break;
                        }
                    case "username":
                        {
                            query = query.Where(x => x.UserName.Contains(keyWord));
                            break;
                        }
                    case "firstname":
                        {
                            query = query.Where(x => x.FirstName.Contains(keyWord));
                            break;
                        }
                    case "lastname":
                        {
                            query = query.Where(x => x.LastName.Contains(keyWord));
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
        #endregion
        private static string GetSignedRequest(string username, string applicationKey, string integrationKey, string secretKey)
        {
            return DuoAuth.SignRequest(integrationKey, secretKey, applicationKey, username);
        }

        private static string? VerifyDuoResponse(string sigResponse, string applicationKey, string integrationKey, string secretKey)
        {
            return DuoAuth.VerifyResponse(integrationKey, secretKey, applicationKey, sigResponse);
        }

        public async Task<Response<bool>> ChangeUserRole(ChangeUserRoleViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.userId.ToString());

                if (user == null)
                {
                    return new Response<bool>() { Data = false, Code = ResponseEnum.AccountNotFound.ResponseCode(), Description = ResponseEnum.AccountNotFound.Description() };
                }

                //get the details of the role

                var role = _dBcontext.Roles.FirstOrDefaultAsync(x => x.Id == Guid.Parse(model.roleId));
                if (role == null)
                {
                    return new Response<bool>() { Data= false, Code = ResponseEnum.RoleNotfound.ResponseCode(), Description = ResponseEnum.RoleNotfound.Description() };
                }

                if (!string.IsNullOrEmpty(model.roleId))
                {
                    user.RoleId = role.Id.ToString();

                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                       
                        return new Response<bool>()
                        {
                            Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                            Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                            Data = true
                        };
                    }
                    else
                    {
                        return new Response<bool>() { Data = false, Code = ResponseEnum.AccountCouldNotBeUpdated.ResponseCode(), Description = ResponseEnum.AccountCouldNotBeUpdated.Description() };
                    }

                  
                }

                return new Response<bool>()
                {
                    Code = ResponseEnum.OperationCompletedSuccesfully.ResponseCode(),
                    Description = ResponseEnum.OperationCompletedSuccesfully.Description(),
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}-An Error Occured, Details: {ex.Message}");
                return new Response<bool>() { Code = ResponseEnum.ErrorOccured.ResponseCode(), Description = ResponseEnum.ErrorOccured.Description() };

            }

        }
    }
}

