
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
public class UserController : ControllerBase
{
    public readonly IUserService _userService;

    public UserController(
        IUserService userService)
    {
        _userService = userService;
    }



    [Route("Invite")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> InviteAsync(InviteUserViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.InviteAsync(model, DateTime.UtcNow));
    }

   
    [Route("SignIn")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<SignInViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignInAsync(AuthCredentialViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.SignInAsync(model, DateTime.UtcNow));
    }

    [Route("RequestPasswordReset")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    internal async Task<IActionResult> RequestPasswordResetAsync(EmailViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.RequestPasswordResetAsync(model, DateTime.UtcNow));
    }

    [Route("ResetPassword")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    internal async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.ResetPasswordAsync(model, DateTime.UtcNow));
    }

    [Route("Update-user/{id}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUserAsync(Guid id, UpdateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.UpdateUserAsync(id, model, DateTime.UtcNow));
    }





    [Route("{id}")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.GetUserAsync(id));
    }


    [Route("GetAllUsers")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<List<ApplicationUserViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllUsers(ExportQueryModel queryModel)
    {
        return Ok(await _userService.GetAllUsers(queryModel));
    }


    [Route("GetCustomerAllUsersAsync")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<ApplicationUserViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCustomerAllUsersAsync(QueryModel queryModel)
    {
        return Ok(await _userService.GetCustomerAllUsersAsync(queryModel));
    }

    [Route("GetAllcustomerUsersByStatus")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<PagedList<ApplicationUserViewModel>>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllcustomerUsersByStatus(QueryModel queryModel, bool isactive)
    {
        return Ok(await _userService.GetAllcustomerUsersByStatus(queryModel, isactive));
    }


  
    //[HttpGet]
    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    //public async Task<IActionResult> GetUsersAsync()
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState.GetApiResponse());

    //    return Ok(await _userService.GetUsersAsync());
    //}

    [HttpGet("get-approvers-users")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetApproversUsersAsync()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.GetApproversUsersAsync());
    }

    //[HttpGet("get-customers-users")]
    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    //public async Task<IActionResult> GetCustomersUsers()
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState.GetApiResponse());

    //    return Ok(await _userService.GetCustomerUsersAsync());
    //}

    [HttpGet("get-superAdmin-users")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuperAdminUsers()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.GetSuperAdminUsersAsync());
    }


    [Route("Disable-a-user/{id}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DisableAUserAsync(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.DisableAsync(id));
    }

    [Route("Enable-a-user/{id}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<ApplicationUserViewModel>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> EnableAUserAsync(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetApiResponse());

        return Ok(await _userService.EnableAsync(id));
    }

    //[Route("Search")]
    //[HttpPost]
    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((typeof(Response<PagedList<ApplicationUserViewModel>>)), (int)HttpStatusCode.OK)]
    //public async Task<IActionResult> SearchAsync(QueryModel queryModel)
    //{
    //    return Ok(await _userService.SearchAsync(queryModel, DateTime.UtcNow));
    //}


    /// <summary>
    /// change user role
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Route("ChanegUserRole")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<bool>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangeUserRole(ChangeUserRoleViewModel model)
    {
        return Ok(await _userService.ChangeUserRole(model));
    }





  

  

 
}
