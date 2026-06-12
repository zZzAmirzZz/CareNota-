using CareNota.DTOs.Auth;
using CareNota.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareNota.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class AuthController(IAuthService AuthService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto Dto)
    {
        try
        {
            var Response = await AuthService.RegisterAsync(Dto);
            return Ok(Response);
        }
        catch (ArgumentException Ex) { return BadRequest(new { Ex.Message }); }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto Dto)
    {
        try
        {
            var Response = await AuthService.LoginAsync(Dto);
            return Ok(Response);
        }
        catch (UnauthorizedAccessException Ex) { return Unauthorized(new { Ex.Message }); }
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto Dto)
    {
        try
        {
            var Response = await AuthService.RefreshTokenAsync(Dto);
            return Ok(Response);
        }
        catch (UnauthorizedAccessException Ex) { return Unauthorized(new { Ex.Message }); }
    }

    [Authorize]
    [HttpPost("Revoke")]
    public async Task<IActionResult> Revoke()
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await AuthService.RevokeTokenAsync(UserId);
        return NoContent();
    }
}