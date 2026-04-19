using Microsoft.AspNetCore.Mvc;
using CareNota.Services.Interfaces;
using CareNota.DTOs.Doctor;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    // 🔹 Get All
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doctors = await _service.GetAllAsync();
        return Ok(doctors);
    }

    // 🔹 Get By Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await _service.GetByIdAsync(id);

        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    // 🔹 Get By Specialty
    [HttpGet("specialty/{specialty}")]
    public async Task<IActionResult> GetBySpecialty(string specialty)
    {
        var doctors = await _service.GetBySpecialtyAsync(specialty);
        return Ok(doctors);
    }

    // 🔹 Update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
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