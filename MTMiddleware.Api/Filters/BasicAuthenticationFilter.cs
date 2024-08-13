using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;

//public class BasicAuthenticationFilter : ActionFilterAttribute
public class BasicAuthenticationFilter : IAsyncActionFilter
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _config;
    public BasicAuthenticationFilter(AppDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("TRANSKEY", out var extractedTransKey))
        {
            context.Result = new UnauthorizedObjectResult("TRANSKEY header not found");
            return;
        }

        try
        {
            //check if the key passed is not active
            var getKey = await _dbContext.CustomersChannelTransKey.Where(x => x.TransKey == extractedTransKey).FirstOrDefaultAsync();
            if (getKey is null)
            {
                context.Result = new UnauthorizedObjectResult("The key passed is invalid");
                return;
            }
            if (!getKey.IsActive)
            {
                context.Result = new UnauthorizedObjectResult("The key passed is Inactive");
                return;
            }
            string cryptoKey = _config.GetValue<string>("AppSettings:CryptoKey");

            //var authHeader = Encoding.UTF8.GetString(Convert.FromBase64String(extractedTransKey.ToString()));
            var authHeader = UtilityLibrary.Common.Utility.Decrypt(extractedTransKey.ToString(), cryptoKey);
            var authHeaderParts = authHeader.Split(':');
            var channel = authHeaderParts[0];
            var customerId = authHeaderParts[1];


            //check if the customerId is Active
            //check if the customerId exists

            var getCustomerDetails = await _dbContext.CustomerDetails.Where(x => x.Id == customerId).FirstOrDefaultAsync();

            if (getCustomerDetails is null)
            {
                context.Result = new UnauthorizedObjectResult("No Customer Has the key passed");
                return;
            }

            if (!getCustomerDetails.IsActive)
            {
                context.Result = new UnauthorizedObjectResult("No Customer with the Key passed is not active, hence cannot process transaction now");
                return;

            }


        }
        catch
        {
            context.Result = new UnauthorizedObjectResult("Invalid TRANSKEY format");
            return;
        }

        await next();
    }
}
