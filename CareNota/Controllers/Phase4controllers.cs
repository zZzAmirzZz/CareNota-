using CareNota.DTOs.Audio;
using CareNota.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareNota.Controllers;

[ApiController]
[Route("api/audio")]
[Authorize]
public class AudioController : ControllerBase
{
    private readonly IAudioService _audioService;

    public AudioController(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Doctor")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AudioRecordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadAudio([FromForm] AudioUploadDto dto)
    {
        try
        {
            var result = await _audioService.UploadAudioAsync(
                dto.AudioFile,
                dto.VisitId);

            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Internal server error",
                Error = ex.Message
            });
        }
    }
    [HttpGet("{visitId:int}/status")]
    [Authorize(Roles = "Doctor")]
    public IActionResult GetStatus([FromRoute] int visitId)
    {
        return Ok(new
        {
            VisitId = visitId,
            SummaryEndpoint = $"/api/visits/{visitId}/summary",
            Message = "Poll the summary endpoint to check AI processing status."
        });
    }

}
