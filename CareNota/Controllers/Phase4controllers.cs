//using CareNota.DTOs.Audio;
//using CareNota.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CareNota.Controllers;

//// ══════════════════════════════════════════════════════════════════════════════
//// AudioController
//// ══════════════════════════════════════════════════════════════════════════════
//[ApiController]
//[Route("Api/[controller]")]
////[Authorize]
//public class AudioController : ControllerBase
//{
//    private readonly IAudioService _AudioService;

//    public AudioController(IAudioService AudioService)
//        => _AudioService = AudioService;

//    // POST Api/Audio/Upload/{visitId}
//    // Doctor uploads the audio recording for a visit
//    [HttpPost("Upload/{VisitId:int}")]
//    //[Authorize(Roles = "Doctor")]
//    [Consumes("multipart/form-data")]
//    public async Task<IActionResult> Upload(int VisitId, [FromForm] AudioUploadDto Dto)
//    {
//        try
//        {
//            var Result = await _AudioService.UploadAsync(VisitId, Dto.AudioFile);
//            return Ok(Result);
//        }
//        catch (KeyNotFoundException Ex) { return NotFound(new { Ex.Message }); }
//        catch (InvalidOperationException Ex) { return Conflict(new { Ex.Message }); }
//    }

//    // GET Api/Audio/Status/{visitId}
//    // Check processing status: "Processing" | "Done"
//    [HttpGet("Status/{VisitId:int}")]
//    //[Authorize(Roles = "Doctor,Receptionist")]
//    public async Task<IActionResult> GetStatus(int VisitId)
//    {
//        var Record = await _AudioService.GetByVisitIdAsync(VisitId);
//        return Record is null
//            ? NotFound(new { Message = $"No audio found for visit {VisitId}." })
//            : Ok(Record);
//    }
//}
