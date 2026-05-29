// AdminController.cs
using CareNota.Services.Interfaces;
using CareNota.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareNota.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _AdminService;

    public AdminController(IAdminService AdminService)
        => _AdminService = AdminService;

    private string CurrentUserId
        => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ── GET api/admin/profile ─────────────────────────────────────────────────

    /// <summary>Returns the current admin's profile.</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var Profile = await _AdminService.GetProfileAsync(CurrentUserId);
        if (Profile is null)
            return NotFound(new { Message = "Admin profile not found." });

        return Ok(Profile);
    }

    // ── PUT api/admin/profile ─────────────────────────────────────────────────

    /// <summary>Updates FullName, PhoneNumber, Gender.</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateAdminProfileDto Dto)
    {
        var Success = await _AdminService.UpdateProfileAsync(CurrentUserId, Dto);
        if (!Success)
            return BadRequest(new { Message = "Profile update failed." });

        return Ok(new { Message = "Profile updated successfully." });
    }

    // ── PUT api/admin/change-password ─────────────────────────────────────────

    /// <summary>Changes the current admin's password.</summary>
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto Dto)
    {
        var (Success, Error) = await _AdminService.ChangePasswordAsync(CurrentUserId, Dto);

        if (!Success)
            return BadRequest(new { Message = Error });

        return Ok(new { Message = "Password changed successfully." });
    }
}