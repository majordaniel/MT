
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
public class DashboardController : ControllerBase
{
    public readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [Route("get-a-adminDashboard")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<AdminDashboardViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAdminDashboardMetrics()
    {
        return Ok(await _dashboardService.GetAdminDashboardDetails());
    }



    //Dashboard
    [Route("get-a-customerdashboardDetailsByAccountNo/{accounNo}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<CustomerDashboardViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetACustomersDashboardDetailsByAccountNo(string accounNo)
    {
        return Ok(await _dashboardService.GetACustomersDashboardDetails(accounNo));
    }


}
