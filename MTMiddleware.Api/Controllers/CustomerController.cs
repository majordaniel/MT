
using MTMiddleware.Data.ViewModels;
using MTMiddleware.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UtilityLibrary.Extensions;
using UtilityLibrary.Models;
using MTMiddleware.Core.Services.Interfaces;
using MTMiddleware.Core.Services;

namespace MTMiddleware.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class CustomerController : ControllerBase
{
    public readonly ICustomerService _customerService;

    public CustomerController(
        ICustomerService customerService)
    {
        _customerService = customerService;
    }





    /// <summary>
    /// Get all Customers Details
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [Route("get-all-customer")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<CustomerDetailsResponseViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllCustomers(QueryModel queryModel)
    {
        return Ok(await _customerService.GetAllCustomersDetails(queryModel));
    }

    /// <summary>
    /// Get all Pending Customers Details
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [Route("get-all-pending-customers")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<CustomerDetailsResponseViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllPendingCustomersDetails(QueryModel queryModel)
    {
        return Ok(await _customerService.GetAllPendingCustomersDetails(queryModel));
    }




    /// <summary>
    /// Get a Customer Detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("get-a-customerdetails/{id}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<CustomerDetailsResponseViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetACustomersDetails(Guid id)
    {
        return Ok(await _customerService.GetACustomersDetails(id));
    }

    /// <summary>
    /// Change customer status ---- Enable or disable customer
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Route("change-customer-status")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangeCustomerStatus(ChangeCustomerStatusViewModel model)
    {
        return Ok(await _customerService.ChangeCustomerStatus(model));
    }

    /// <summary>
    /// Enable customer and update the customer transaction limit for SWIFT and RTGS
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Route("UpdateCustomerApprovalAndLimit")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateCustomerApprovalAndLimit(UpdateCustomerApprovalAndLimit model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _customerService.UpdateCustomerApprovalAndLimit(model));
    }
    /// <summary>
    /// Register a new Customer 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>

    [Route("Register-Customer")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterCustomer(RegisterCustomerRequestViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _customerService.RegisterCustomerAsync(model, DateTime.UtcNow));
    }



}
