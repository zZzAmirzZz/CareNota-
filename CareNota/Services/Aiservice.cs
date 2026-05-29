using CareNota.DTOs.Summary;
using CareNota.Interfaces;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace CareNota.Services;

public class AIService : IAIService
{
    private readonly HttpClient _HttpClient;
    private readonly IAISummaryRepository _AISummaryRepository;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AIService> _Logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AIService(
        HttpClient HttpClient,
        IAISummaryRepository AISummaryRepository,
        IConfiguration Configuration,
        ILogger<AIService> Logger)
    {
        _HttpClient = HttpClient;
        _AISummaryRepository = AISummaryRepository;
        _Configuration = Configuration;
        _Logger = Logger;
    }

    public async Task ProcessAudioAsync(string AudioUrl, int VisitId)
    {
        // ── 1. Build request: { "audio_url": "..." } ─────────────────────────
        var Content = new StringContent(
            JsonSerializer.Serialize(new { audio_url = AudioUrl }),
            Encoding.UTF8,
            "application/json");

        var Endpoint = $"{_Configuration["AIService:BaseUrl"]?.TrimEnd('/')}/process-audio";

        _Logger.LogInformation(
            "Calling AI service. VisitId={VisitId} | Endpoint={Endpoint}", VisitId, Endpoint);

        // ── 2. Call FastAPI ───────────────────────────────────────────────────
        HttpResponseMessage Response;
        try
        {
            Response = await _HttpClient.PostAsync(Endpoint, Content);
            Response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException Ex)
        {
            _Logger.LogError(Ex, "AI service call failed for VisitId {VisitId}.", VisitId);
            throw;
        }

        // ── 3. Deserialise ────────────────────────────────────────────────────
        var Json = await Response.Content.ReadAsStringAsync();
        var Result = JsonSerializer.Deserialize<AIProcessResponseDto>(Json, JsonOptions);

        if (!string.IsNullOrWhiteSpace(Result?.Error))
        {
            _Logger.LogError("AI error for VisitId {VisitId}: {Error}", VisitId, Result.Error);
            throw new InvalidOperationException($"AI error: {Result.Error}");
        }

        // ── 4. Save two DRAFT rows ────────────────────────────────────────────
        //
        //  "Doctor"  → { subjective, objective, assessment, plan }
        //  "Patient" → { diagnosis, symptoms, treatmentPlan, whenToSeekHelp, followUp }
        //
        //  Both are drafts. Doctor reviews via GET /summary,
        //  edits via PUT /summary, finalises via POST /summary/approve.
        //  On approve → SOAP + WhenToSeekHelp + FollowUpDate written to Visit.

        if (Result?.DoctorSummary is not null)
            await _AISummaryRepository.AddAsync(new AISummary
            {
                SummaryType = "Doctor",
                SummaryText = JsonSerializer.Serialize(Result.DoctorSummary),
                VisitID = VisitId
            });

        if (Result?.PatientSummary is not null)
            await _AISummaryRepository.AddAsync(new AISummary
            {
                SummaryType = "Patient",
                SummaryText = JsonSerializer.Serialize(Result.PatientSummary),
                VisitID = VisitId
            });

        await _AISummaryRepository.SaveAsync();

        _Logger.LogInformation("Draft summaries saved for VisitId {VisitId}.", VisitId);
    }
}