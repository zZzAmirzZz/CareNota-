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
    private readonly IVisitRepository _VisitRepository;      
    private readonly IPatientRepository _PatientRepository; 
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AIService> _Logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AIService(
        HttpClient HttpClient,
        IAISummaryRepository AISummaryRepository,
        IVisitRepository VisitRepository,
        IPatientRepository PatientRepository,
        IConfiguration Configuration,
        ILogger<AIService> Logger)
    {
        _HttpClient = HttpClient;
        _AISummaryRepository = AISummaryRepository;
        _VisitRepository = VisitRepository;
        _PatientRepository = PatientRepository;
        _Configuration = Configuration;
        _Logger = Logger;

        if (_Configuration.GetValue<bool>("AIService:SkipNgrokWarning"))
            _HttpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
    }

    public async Task ProcessAudioAsync(string AudioUrl, int VisitId)
    {
        // ── 1. Load current visit to get AppointmentID → PatientID ───────────
        var Visit = await _VisitRepository.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        // ── 2. Load patient for context fields ───────────────────────────────
        var Patient = await _PatientRepository.GetPatientByVisitIdAsync(VisitId);

        // ── 3. Calculate age from DateOfBirth ─────────────────────────────────
        int? Age = null;
        if (Patient?.DateOfBirth is not null)
        {
            var Today = DateTime.Today;
            Age = Today.Year - Patient.DateOfBirth.Value.Year;
            if (Patient.DateOfBirth.Value.Date > Today.AddYears(-Age.Value)) Age--;
        }

        // ── 4. Get last approved summary for this patient (if any) ────────────
        //
        //  "last_summary" gives the AI context about the patient's previous visit
        //  so it can generate a meaningful comparison_with_previous_visit field.
        //  We fetch the most recent Doctor-type AISummary for any previous visit
        //  belonging to the same patient, excluding the current visit.
        object? LastSummary = null;

        var PreviousDoctorSummary = await _AISummaryRepository
            .GetLastApprovedDoctorSummaryByPatientAsync(
                Patient?.PatientID ?? 0,
                ExcludeVisitId: VisitId);

        if (PreviousDoctorSummary is not null)
        {
            // Deserialize the stored JSON blob and pass it as-is to the AI
            // so it gets the full SOAP structure of the previous visit
            try
            {
                LastSummary = JsonSerializer.Deserialize<object>(
                    PreviousDoctorSummary.SummaryText, JsonOptions);
            }
            catch
            {
                // If deserialization fails for any reason, send null — not fatal
                LastSummary = null;
            }
        }




        // ── 5. Build the request payload ──────────────────────────────────────
        var RequestPayload = new
        {
            audio_url = AudioUrl,
            age = Age,
            gender = Patient?.Gender,
            chronic_conditions = Patient?.ChronicConditions,
            allergies = Patient?.Allergies,
            last_summary = LastSummary   // null if first visit
        };
        var jsonPayload = JsonSerializer.Serialize(RequestPayload);

        _Logger.LogInformation(
            "AI Request Payload: {Payload}",
            jsonPayload);


        var Content = new StringContent(
            JsonSerializer.Serialize(RequestPayload),
            Encoding.UTF8,
            "application/json");

        var Endpoint = $"{_Configuration["AIService:BaseUrl"]?.TrimEnd('/')}/process-audio";

        _Logger.LogInformation(
            "Calling AI service. VisitId={VisitId} | Endpoint={Endpoint} | " +
            "Age={Age} | HasLastSummary={HasLastSummary}",
            VisitId, Endpoint, Age, LastSummary is not null);

        // ── 6. Call FastAPI ───────────────────────────────────────────────────
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

        // ── 7. Deserialise ────────────────────────────────────────────────────
        var Json = await Response.Content.ReadAsStringAsync();
        var Result = JsonSerializer.Deserialize<AIProcessResponseDto>(Json, JsonOptions);

        if (!string.IsNullOrWhiteSpace(Result?.Error))
        {
            _Logger.LogError("AI error for VisitId {VisitId}: {Error}", VisitId, Result.Error);
            throw new InvalidOperationException($"AI error: {Result.Error}");
        }

        // ── 8. Save two DRAFT rows ────────────────────────────────────────────
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