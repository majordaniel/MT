//using MTMiddleware.Core.Commands.User;

//using MTMiddleware.Core.Queries.User;
using MTMiddleware.Data.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UtilityLibrary.Extensions;
using UtilityLibrary.Models;
using MTMiddleware.Core.Services.Interfaces;

namespace MTMiddleware.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class RoleController : ControllerBase
{
    public readonly IRoleService _roleService;

    public RoleController(
        IRoleService roleService)
    {
        _roleService = roleService;
    }
    /// <summary>
    /// getr
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    
    //[Route("ger-role/{id}")]
    //[HttpGet]
    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((typeof(Response<ApplicationRoleViewModel>)), (int)HttpStatusCode.OK)]

    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ApplicationRoleViewModel>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Response<string>))]
    [HttpGet, Route("get-role")]

    public async Task<IActionResult> GetRoleAsync(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _roleService.GetItemAsync(id));
    }


    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<List<ApplicationRoleViewModel>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Response<string>))]
    [HttpGet, Route("get-roles")]

    public async Task<IActionResult> GetRolesAsync()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _roleService.GetRolesAsync());
    }

    //[Route("ExportData")]
    //[HttpPost]
    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((typeof(Response<List<ApplicationUserViewModel>>)), (int)HttpStatusCode.OK)]

    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<List<ApplicationRoleViewModel>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Response<string>))]
    [HttpPost, Route("export-data")]

    public async Task<IActionResult> ExportData(ExportQueryModel queryModel)
    {
        return Ok(await _roleService.ExportDataAsync(queryModel));
    }
}
