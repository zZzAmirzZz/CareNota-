// AdminController.cs
using CareNota.DTOs.Admin;
using CareNota.Services;
using CareNota.Services.Interfaces;
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

    /// <summary>Creates a new doctor account.</summary>
    [HttpPost("create-doctor")]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _AdminService.CreateDoctorAccountAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Creates a new receptionist account.</summary>
    [HttpPost("create-receptionist")]
    public async Task<IActionResult> CreateReceptionist([FromBody] CreateReceptionistDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _AdminService.CreateReceptionistAccountAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
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
    // ── Receptionists ──────────────────────────────────────────

    [HttpGet("receptionists")]
    public async Task<IActionResult> GetAllReceptionists()
    {
        var receptionists = await _AdminService.GetAllReceptionistsAsync();
        return Ok(receptionists);
    }

    [HttpDelete("receptionists/{id}")]
    public async Task<IActionResult> DeleteReceptionist(string id)
    {
        var result = await _AdminService.DeleteReceptionistAsync(id);
        if (!result)
            return NotFound(new { message = "Receptionist not found." });

        return NoContent();
    }
}