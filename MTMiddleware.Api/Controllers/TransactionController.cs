
using MTMiddleware.Data.ViewModels;
using MTMiddleware.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UtilityLibrary.Extensions;
using UtilityLibrary.Models;
using MTMiddleware.Core.Services.Interfaces;

namespace MTMiddleware.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TransactionController : ControllerBase
{
    public readonly ITransactionService _transactionService;

    public TransactionController(
        ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Endpoint to send Transaction requests--- Required: Header parameter ("TRANSKEY")
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>

    [ServiceFilter(typeof(BasicAuthenticationFilter))]
    [Route("Send-Transaction")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<CreateTransactionResponse>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateTransaction(CreateTransactionRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _transactionService.CreateTransaction(model));
    }

    /// <summary>
    /// Get all Swift transactions by a customer
    /// </summary>
    /// <param name="queryModel"></param>
    /// <param name="CustomerId"></param>
    /// <returns></returns>
    // Transaction 
    [Route("GetAllCustomerSwiftTransaction/{CustomerId}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<CustomerTransactionResponseViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllCustomerSwiftTransaction(DateRangeQueryModel queryModel, string CustomerId)
    {
        return Ok(await _transactionService.GetAllCustomerSwiftTransaction(queryModel, CustomerId));
    }


    /// <summary>
    /// Get all RTGS transactions by a customer
    /// </summary>
    /// <param name="queryModel"></param>
    /// <param name="CustomerId"></param>
    /// <returns></returns>
    [Route("GetAllCustomerRTGSTransaction/{CustomerId}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<CustomerTransactionResponseViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllCustomerRTGSTransaction(DateRangeQueryModel queryModel, string CustomerId)
    {
        return Ok(await _transactionService.GetAllCustomerRTGSTransaction(queryModel, CustomerId));
    }



}
