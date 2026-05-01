using Microsoft.AspNetCore.Mvc;
using CareNota.Services.Interfaces;
using CareNota.DTOs.Appointment;
using CareNota.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace CareNota.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IValidator<CreateAppointmentDto> _createValidator;
    private readonly IValidator<UpdateAppointmentDto> _updateValidator;

    public AppointmentController(
        IAppointmentService appointmentService,
        IValidator<CreateAppointmentDto> createValidator,
        IValidator<UpdateAppointmentDto> updateValidator)
    {
        _appointmentService = appointmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // ── READ ENDPOINTS ─────────────────────────────────────────────────────

    [HttpGet]
    //[Authorize]                                   // Any authenticated user
    public async Task<IActionResult> GetAll()
    {
        var data = await _appointmentService.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _appointmentService.GetByIdAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpGet("{id}/details")]
    //[Authorize]
    public async Task<IActionResult> GetDetails(int id)
    {
        var data = await _appointmentService.GetDetailsAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpGet("patient/{patientId}")]
    //[Authorize(Roles = "Patient,Doctor,Receptionist,Admin")]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var data = await _appointmentService.GetByPatientIdAsync(patientId);
        return Ok(data);
    }

    [HttpGet("doctor/{doctorId}")]
    //[Authorize(Roles = "Doctor,Receptionist,Admin")]
    public async Task<IActionResult> GetByDoctor(int doctorId)
    {
        var data = await _appointmentService.GetByDoctorIdAsync(doctorId);
        return Ok(data);
    }

    // ── Get By Status (Corrected & Improved) ─────────────────────────────
    [HttpGet("status/{status}")]
    //[Authorize(Roles = "Receptionist,Admin,Doctor")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        if (!Enum.TryParse<AppointmentStatus>(status, true, out var appointmentStatus))
        {
            return BadRequest("Invalid status. Allowed values: Scheduled, Completed, Cancelled");
        }

        var data = await _appointmentService.GetByStatusAsync(appointmentStatus);
        return Ok(data);
    }

    [HttpGet("date-range")]
    //[Authorize(Roles = "Receptionist,Admin,Doctor")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        try
        {
            var data = await _appointmentService.GetByDateRangeAsync(from, to);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("doctor/{doctorId}/weekly")]
    //[Authorize(Roles = "Doctor,Receptionist,Admin")]
    public async Task<IActionResult> GetWeeklySchedule(int doctorId, [FromQuery] DateTime startOfWeek)
    {
        var data = await _appointmentService.GetDoctorWeeklyScheduleAsync(doctorId, startOfWeek);
        return Ok(data);
    }

    [HttpGet("doctor/{doctorId}/available-slots")]
    //[Authorize(Roles = "Receptionist,Admin")]
    public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
    {
        var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
        return Ok(slots);
    }

    // ── WRITE ENDPOINTS ────────────────────────────────────────────────────

    [HttpPost]
    //[Authorize(Roles = "Receptionist")]        // Usually only Receptionist/Admin can create
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var created = await _appointmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentID }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging (Serilog)
            return StatusCode(500, "An error occurred while creating the appointment.");
        }
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = "Receptionist,Admin,Doctor")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var updated = await _appointmentService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}/cancel")]
    //[Authorize(Roles = "Receptionist,Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _appointmentService.CancelAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]        // Only Admin should hard delete
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}