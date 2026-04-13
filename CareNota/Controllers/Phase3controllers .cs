using CareNota.DTOs.Diagnosis;
using CareNota.DTOs.LabTest;
using CareNota.DTOs.Medication;
using CareNota.DTOs.Prescription;
using CareNota.DTOs.Visit;
using CareNota.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareNota.Controllers;

// ══════════════════════════════════════════════════════════════════════════════
// VisitController
// ══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class VisitController : ControllerBase
{
    private readonly IVisitService _Service;

    public VisitController(IVisitService Service) => _Service = Service;

    // GET Api/Visit
    [HttpGet]
    [Authorize(Roles = "Doctor,Receptionist")]
    public async Task<IActionResult> GetAll()
        => Ok(await _Service.GetAllAsync());

    // GET Api/Visit/{id}
    [HttpGet("{Id:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetById(int Id)
    {
        var Visit = await _Service.GetByIdAsync(Id);
        return Visit is null ? NotFound(new { Message = $"Visit {Id} not found." }) : Ok(Visit);
    }

    // GET Api/Visit/{id}/Details
    [HttpGet("{Id:int}/Details")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetDetails(int Id)
    {
        var Visit = await _Service.GetDetailsAsync(Id);
        return Visit is null ? NotFound(new { Message = $"Visit {Id} not found." }) : Ok(Visit);
    }

    // GET Api/Visit/Patient/{patientId}
    [HttpGet("Patient/{PatientId:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetByPatient(int PatientId)
        => Ok(await _Service.GetByPatientIdAsync(PatientId));

    // GET Api/Visit/Appointment/{appointmentId}
    [HttpGet("Appointment/{AppointmentId:int}")]
    [Authorize(Roles = "Doctor,Receptionist")]
    public async Task<IActionResult> GetByAppointment(int AppointmentId)
    {
        var Visit = await _Service.GetByAppointmentIdAsync(AppointmentId);
        return Visit is null
            ? NotFound(new { Message = $"No visit found for appointment {AppointmentId}." })
            : Ok(Visit);
    }

    // POST Api/Visit
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateVisitDto Dto)
    {
        try
        {
            var Created = await _Service.CreateAsync(Dto);
            return CreatedAtAction(nameof(GetById), new { Id = Created.VisitID }, Created);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // PUT Api/Visit/{id}
    [HttpPut("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Update(int Id, [FromBody] UpdateVisitDto Dto)
    {
        try
        {
            var Updated = await _Service.UpdateAsync(Id, Dto);
            return Ok(Updated);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // DELETE Api/Visit/{id}
    [HttpDelete("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            await _Service.DeleteAsync(Id);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// DiagnosisController
// ══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class DiagnosisController : ControllerBase
{
    private readonly IDiagnosisService _Service;

    public DiagnosisController(IDiagnosisService Service) => _Service = Service;

    // GET Api/Diagnosis
    [HttpGet]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetAll()
        => Ok(await _Service.GetAllAsync());

    // GET Api/Diagnosis/{icdCode}   e.g. Api/Diagnosis/J18.9
    [HttpGet("{IcdCode}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetByCode(string IcdCode)
    {
        var Diagnosis = await _Service.GetByIcdCodeAsync(IcdCode);
        return Diagnosis is null
            ? NotFound(new { Message = $"ICD-10 code '{IcdCode}' not found." })
            : Ok(Diagnosis);
    }

    // GET Api/Diagnosis/Search?query=pneumonia
    [HttpGet("Search")]
    [Authorize(Roles = "Doctor,Receptionist")]
    public async Task<IActionResult> Search([FromQuery] string Query)
    {
        if (string.IsNullOrWhiteSpace(Query))
            return BadRequest(new { Message = "Search query is required." });
        return Ok(await _Service.SearchAsync(Query));
    }

    // GET Api/Diagnosis/Visit/{visitId}
    [HttpGet("Visit/{VisitId:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetByVisit(int VisitId)
        => Ok(await _Service.GetByVisitIdAsync(VisitId));

    // POST Api/Diagnosis  ← add a new ICD-10 code to the system catalog
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateDiagnosisDto Dto)
    {
        try
        {
            var Created = await _Service.CreateAsync(Dto);
            return CreatedAtAction(nameof(GetByCode), new { IcdCode = Created.ICD10Code }, Created);
        }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // POST Api/Diagnosis/Visit/{visitId}/Assign  ← link ICD-10 to a visit
    [HttpPost("Visit/{VisitId:int}/Assign")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> AssignToVisit(
        int VisitId, [FromBody] AssignDiagnosisToVisitDto Dto)
    {
        try
        {
            await _Service.AssignToVisitAsync(VisitId, Dto);
            return Ok(new { Message = $"Diagnosis '{Dto.ICD10Code}' assigned to visit {VisitId}." });
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // DELETE Api/Diagnosis/Visit/{visitId}/{icdCode}  ← unlink from visit
    [HttpDelete("Visit/{VisitId:int}/{IcdCode}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> RemoveFromVisit(int VisitId, string IcdCode)
    {
        try
        {
            await _Service.RemoveFromVisitAsync(VisitId, IcdCode);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // DELETE Api/Diagnosis/{icdCode}  ← remove from catalog entirely
    [HttpDelete("{IcdCode}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Delete(string IcdCode)
    {
        try
        {
            await _Service.DeleteAsync(IcdCode);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// PrescriptionController
// ══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _Service;

    public PrescriptionController(IPrescriptionService Service) => _Service = Service;

    // GET Api/Prescription/{id}
    [HttpGet("{Id:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetById(int Id)
    {
        var Prescription = await _Service.GetByIdAsync(Id);
        return Prescription is null
            ? NotFound(new { Message = $"Prescription {Id} not found." })
            : Ok(Prescription);
    }

    // GET Api/Prescription/Visit/{visitId}
    [HttpGet("Visit/{VisitId:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetByVisit(int VisitId)
    {
        var Prescription = await _Service.GetByVisitIdAsync(VisitId);
        return Prescription is null
            ? NotFound(new { Message = $"No prescription found for visit {VisitId}." })
            : Ok(Prescription);
    }

    // POST Api/Prescription
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto Dto)
    {
        try
        {
            var Created = await _Service.CreateAsync(Dto);
            return CreatedAtAction(nameof(GetById), new { Id = Created.PrescriptionID }, Created);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // PUT Api/Prescription/{id}
    [HttpPut("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Update(int Id, [FromBody] UpdatePrescriptionDto Dto)
    {
        try
        {
            var Updated = await _Service.UpdateAsync(Id, Dto);
            return Ok(Updated);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // POST Api/Prescription/{id}/Medications  ← add a medication to prescription
    [HttpPost("{Id:int}/Medications")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> AddMedication(
        int Id, [FromBody] AddMedicationToPrescriptionDto Dto)
    {
        try
        {
            var Result = await _Service.AddMedicationAsync(Id, Dto);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // DELETE Api/Prescription/{id}/Medications/{medicationId}
    [HttpDelete("{Id:int}/Medications/{MedicationId:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> RemoveMedication(int Id, int MedicationId)
    {
        try
        {
            var Result = await _Service.RemoveMedicationAsync(Id, MedicationId);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // DELETE Api/Prescription/{id}
    [HttpDelete("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            await _Service.DeleteAsync(Id);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// MedicationController
// ══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class MedicationController : ControllerBase
{
    private readonly IMedicationService _Service;

    public MedicationController(IMedicationService Service) => _Service = Service;

    // GET Api/Medication
    [HttpGet]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetAll()
        => Ok(await _Service.GetAllAsync());

    // GET Api/Medication/{id}
    [HttpGet("{Id:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetById(int Id)
    {
        var Medication = await _Service.GetByIdAsync(Id);
        return Medication is null
            ? NotFound(new { Message = $"Medication {Id} not found." })
            : Ok(Medication);
    }

    // GET Api/Medication/Search?name=amoxicillin
    [HttpGet("Search")]
    [Authorize(Roles = "Doctor,Receptionist")]
    public async Task<IActionResult> Search([FromQuery] string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return BadRequest(new { Message = "Name query is required." });
        return Ok(await _Service.SearchByNameAsync(Name));
    }

    // GET Api/Medication/Type/{type}   e.g. Tablet, Capsule
    [HttpGet("Type/{Type}")]
    [Authorize(Roles = "Doctor,Receptionist")]
    public async Task<IActionResult> GetByType(string Type)
        => Ok(await _Service.GetByTypeAsync(Type));

    // POST Api/Medication
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateMedicationDto Dto)
    {
        try
        {
            var Created = await _Service.CreateAsync(Dto);
            return CreatedAtAction(nameof(GetById), new { Id = Created.MedicationID }, Created);
        }
        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
    }

    // PUT Api/Medication/{id}
    [HttpPut("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Update(int Id, [FromBody] UpdateMedicationDto Dto)
    {
        try
        {
            var Updated = await _Service.UpdateAsync(Id, Dto);
            return Ok(Updated);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // DELETE Api/Medication/{id}
    [HttpDelete("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            await _Service.DeleteAsync(Id);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// LabTestController
// ══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class LabTestController : ControllerBase
{
    private readonly ILabTestService _Service;
    private readonly IWebHostEnvironment _Env;

    public LabTestController(ILabTestService Service, IWebHostEnvironment Env)
    {
        _Service = Service;
        _Env = Env;
    }

    // GET Api/LabTest/{id}
    [HttpGet("{Id:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetById(int Id)
    {
        var LabTest = await _Service.GetByIdAsync(Id);
        return LabTest is null
            ? NotFound(new { Message = $"Lab test {Id} not found." })
            : Ok(LabTest);
    }

    // GET Api/LabTest/Visit/{visitId}
    [HttpGet("Visit/{VisitId:int}")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> GetByVisit(int VisitId)
        => Ok(await _Service.GetByVisitIdAsync(VisitId));

    // POST Api/LabTest  ← order a new lab test
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateLabTestDto Dto)
    {
        try
        {
            var Created = await _Service.CreateAsync(Dto);
            return CreatedAtAction(nameof(GetById), new { Id = Created.LabTestID }, Created);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }

    // POST Api/LabTest/{id}/UploadResult  ← upload result file (PDF/JPG/PNG)
    [HttpPost("{Id:int}/UploadResult")]
    [Authorize(Roles = "Doctor,Receptionist")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadResult(int Id, [FromForm] UploadLabResultDto Dto)
    {
        try
        {
            var UploadFolder = Path.Combine(
                _Env.WebRootPath ?? _Env.ContentRootPath, "LabResults");

            var Result = await _Service.UploadResultAsync(Id, Dto.ResultFile, UploadFolder);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
        catch (ArgumentException Ex) { return BadRequest(new { Ex.Message }); }
    }

    // GET Api/LabTest/{id}/Download  ← stream the result file to client
    [HttpGet("{Id:int}/Download")]
    [Authorize(Roles = "Doctor,Receptionist,Patient")]
    public async Task<IActionResult> Download(int Id)
    {
        var LabTest = await _Service.GetByIdAsync(Id);

        if (LabTest is null)
            return NotFound(new { Message = $"Lab test {Id} not found." });

        if (string.IsNullOrEmpty(LabTest.TestResultURL))
            return NotFound(new { Message = "No result file uploaded yet." });

        if (!System.IO.File.Exists(LabTest.TestResultURL))
            return NotFound(new { Message = "Result file not found on server." });

        var Extension = Path.GetExtension(LabTest.TestResultURL).ToLowerInvariant();
        var ContentType = Extension switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        var FileBytes = await System.IO.File.ReadAllBytesAsync(LabTest.TestResultURL);
        var DownloadName = $"{LabTest.LabTestName.Replace(" ", "_")}_Result{Extension}";

        return File(FileBytes, ContentType, DownloadName);
    }

    // DELETE Api/LabTest/{id}
    [HttpDelete("{Id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            await _Service.DeleteAsync(Id);
            return NoContent();
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
    }
}