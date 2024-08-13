
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
public class UtilityController : ControllerBase
{
    public readonly IUtilityService _utilityService;

    public UtilityController(
        IUtilityService utilityService)
    {
        _utilityService = utilityService;
    }

    [Route("DecryptText")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<string>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DecryptText(string EncryptedText)
    {
        return Ok(await _utilityService.DecryptAsync(EncryptedText));
    }

    [Route("EncryptText")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((typeof(Response<string>)), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> EncryptText(string TextToEncrypte)
    {
        return Ok(await _utilityService.EncryptAsync(TextToEncrypte));
    }



}
