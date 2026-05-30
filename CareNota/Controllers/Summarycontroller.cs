using CareNota.DTOs.Summary;
using CareNota.Interfaces;
using CareNota.Models;
using CareNota.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math;

namespace CareNota.Controllers;
//GET  /api/visits/{id}/ summary          → Doctor reviews draft
//PUT  /api/visits/{id}/ summary          → Doctor edits(null fields preserved)
//POST / api / visits /{ id}/ summary / approve  → Writes everything to Visit { followUpDate? }
//GET / api / visits /{ id}/ patient - summary  → Patient reads(400 if not approved yet)

[ApiController]
[Route("api/visits/{VisitId:int}")]
//[Authorize]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _SummaryService;

    public SummaryController(ISummaryService SummaryService)
    {
        _SummaryService = SummaryService;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DOCTOR SIDE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// GET /api/visits/{visitId}/summary
    ///
    /// Returns the AI draft for the doctor to review.
    /// Returns 404 while AI is still processing (poll until 200).
    ///
    /// Response includes:
    ///   - isApproved: false (draft) or true (already approved)
    ///   - doctorSummary: SOAP fields (Subjective, Objective, Assessment, Plan)
    ///   - patientSummary: Arabic fields (Diagnosis, Symptoms, TreatmentPlan, WhenToSeekHelp)
    /// </summary>
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

    /// <summary>
    /// PUT /api/visits/{visitId}/summary
    ///
    /// Doctor edits the draft before approving.
    /// Send ONLY the fields that changed — null fields are kept as-is.
    ///
    /// Editable fields:
    ///   - subjective, objective, assessment, plan  (SOAP)
    ///   - whenToSeekHelp                           (patient Arabic text)
    /// </summary>
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

    /// <summary>
    /// POST /api/visits/{visitId}/summary/approve
    ///
    /// Doctor finalises the summary. This action:
    ///   1. Writes SOAP fields (Subjective, Objective, Assessment, Plan) into the Visit row
    ///   2. Writes WhenToSeekHelp into the Visit row
    ///   3. Writes FollowUpDate into the Visit row (set by doctor, not AI)
    ///
    /// After this call the patient can see their summary via GET /patient-summary.
    ///
    /// Body: { "followUpDate": "2026-06-15T00:00:00Z" }  (followUpDate is optional)
    /// </summary>
    [HttpPost("summary/approve")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveSummary(
        [FromRoute] int VisitId,
        [FromBody] ApproveSummaryDto Dto)
    {
        try
        {
            await _SummaryService.ApproveSummaryAsync(VisitId, Dto);
            return Ok(new { Message = "Summary approved. Patient summary is now visible to the patient." });
        }
        catch (KeyNotFoundException Ex)
        {
            return NotFound(new { Message = Ex.Message });
        }
        catch (InvalidOperationException Ex)
        {
            return BadRequest(new { Message = Ex.Message });
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PATIENT SIDE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// GET /api/visits/{visitId}/patient-summary
    ///
    /// Returns the patient-facing Arabic summary.
    /// Only available AFTER the doctor has approved (returns 400 otherwise).
    ///
    /// Response includes:
    ///   - diagnosis, symptoms, treatmentPlan  (from AISummary, edited by doctor)
    ///   - whenToSeekHelp                      (from Visit, written on approval)
    ///   - followUpDate                        (from Visit, set by doctor on approval)
    /// </summary>
    [HttpGet("patient-summary")]
    [Authorize(Roles = "Patient,Doctor")]
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
        catch (KeyNotFoundException Ex)
        {
            return NotFound(new { Message = Ex.Message });
        }
        catch (InvalidOperationException Ex)
        {
            // Doctor has not approved yet
            return BadRequest(new { Message = Ex.Message });
        }
    }
}