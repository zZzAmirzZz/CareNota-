using CareNota.DTOs.Summary;
using CareNota.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareNota.Controllers;

[ApiController]
[Route("api/visits/{VisitId:int}")]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _SummaryService;

    public SummaryController(ISummaryService SummaryService)
        => _SummaryService = SummaryService;

    // ── GET /api/visits/{visitId}/summary ────────────────────────────────────
    // Doctor reviews AI draft. Returns 404 while AI still processing (poll until 200).
    [HttpGet("summary")]
    //[Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(VisitSummaryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary([FromRoute] int VisitId)
    {
        try
        {
            var Result = await _SummaryService.GetSummaryAsync(VisitId);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex)
        {
            return NotFound(new { Message = Ex.Message });
        }
    }

    // ── PUT /api/visits/{visitId}/summary ────────────────────────────────────
    // Doctor edits draft before approving. Null fields are preserved.
    // Editable: subjective, objective, assessment, plan (SOAP)
    //           diagnosis, symptoms, treatmentPlan, whenToSeekHelp (patient Arabic)
    [HttpPut("summary")]
    //[Authorize(Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditSummary(
        [FromRoute] int VisitId,
        [FromBody] EditSummaryDto Dto)
    {
        try
        {
            await _SummaryService.EditSummaryAsync(VisitId, Dto);
            return NoContent();
        }
        catch (KeyNotFoundException Ex)
        {
            return NotFound(new { Message = Ex.Message });
        }
    }

    // ── POST /api/visits/{visitId}/summary/approve ───────────────────────────
    // Finalises the summary:
    //   1. SOAP fields → Visit columns
    //   2. Symptoms + WhenToSeekHelp → Visit columns
    //   3. FollowUpDate → Visit.FollowUpDate (set by doctor)
    //   4. Diagnosis → Diagnosis table row
    //   5. TreatmentPlan → Prescription.Instructions (created if needed)
    [HttpPost("summary/approve")]
    //[Authorize(Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveSummary(int VisitId)
    {
        try
        {
            await _SummaryService.ApproveSummaryAsync(VisitId);
            return Ok(new { Message = "Summary approved. Patient summary is now visible." });
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Message = Ex.Message }); }
        catch (InvalidOperationException Ex) { return BadRequest(new { Message = Ex.Message }); }
    }

    // ── GET /api/visits/{visitId}/patient-summary ────────────────────────────
    // Patient reads their Arabic summary — only after doctor approves.
    // Returns 400 if not yet approved.
    [HttpGet("patient-summary")]
    //[Authorize(Roles = "Patient,Doctor")]
    [ProducesResponseType(typeof(PatientSummaryViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatientSummary([FromRoute] int VisitId)
    {
        try
        {
            var Result = await _SummaryService.GetPatientSummaryAsync(VisitId);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex) { return NotFound(new { Message = Ex.Message }); }
        catch (InvalidOperationException Ex) { return BadRequest(new { Message = Ex.Message }); }
    }

    // ── POST /api/visits/{visitId}/summary/rating ────────────────────────────
    // Optional — doctor rates AI summary quality (1–5) with optional feedback.
    // Used for future model improvement. Can be called any time after generation.
    [HttpPost("summary/rating")]
    //[Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(RateSummaryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateSummary(
        [FromRoute] int VisitId,
        [FromBody] RateSummaryDto Dto)
    {
        try
        {
            var Result = await _SummaryService.RateSummaryAsync(VisitId, Dto);
            return Ok(Result);
        }
        catch (KeyNotFoundException Ex)
        {
            return NotFound(new { Message = Ex.Message });
        }
    }
}