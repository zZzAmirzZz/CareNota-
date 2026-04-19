using Microsoft.AspNetCore.Mvc;
using CareNota.Services.Interfaces;
using CareNota.DTOs.Appointment;

[ApiController]
[Route("api/[controller]")]

public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentController(IAppointmentService service)
    {
        _service = service;
    }

    // 🔹 Get All
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }

    // 🔹 Get By Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);

        if (data == null)
            return NotFound();

        return Ok(data);
    }

    // 🔹 Get Full Details
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var data = await _service.GetDetailsAsync(id);

        if (data == null)
            return NotFound();

        return Ok(data);
    }

    // 🔹 Get By Patient
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var data = await _service.GetByPatientIdAsync(patientId);
        return Ok(data);
    }

    // 🔹 Get By Status
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _service.GetByStatusAsync(status);
        return Ok(data);
    }

    // 🔹 Date Range
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange(DateTime from, DateTime to)
    {
        try
        {
            var data = await _service.GetByDateRangeAsync(from, to);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 🔹 Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 🔹 Update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAppointmentDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 🔹 Cancel
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _service.CancelAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 🔹 Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}