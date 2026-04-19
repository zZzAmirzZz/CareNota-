using Microsoft.AspNetCore.Mvc;
using CareNota.Services.Interfaces;
using CareNota.DTOs.Patient;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientController(IPatientService service)
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

    // 🔹 Details
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var data = await _service.GetDetailsAsync(id);

        if (data == null)
            return NotFound();

        return Ok(data);
    }

    // 🔹 Search
    [HttpGet("search")]
    public async Task<IActionResult> Search(string name)
    {
        var result = await _service.SearchByNameAsync(name);
        return Ok(result);
    }

    // 🔹 Update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdatePatientDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}