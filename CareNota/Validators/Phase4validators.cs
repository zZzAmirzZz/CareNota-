using CareNota.DTOs;
using CareNota.DTOs.Audio;
using FluentValidation;

namespace CareNota.BLL.Validators;

public class AudioUploadValidator : AbstractValidator<AudioUploadDto>
{
    // Allowed audio MIME types accepted by the FastAPI side
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "audio/wav",
        "audio/x-wav",
        "audio/mpeg",       // mp3
        "audio/mp3",
        "audio/x-m4a",      // m4a
        "audio/mp4",
        "audio/m4a"
    };

    // Allowed extensions as a second guard layer
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".wav", ".mp3", ".m4a"
    };

    private const long MaxFileSizeBytes = 52_428_800; // 50 MB

    public AudioUploadValidator()
    {
        RuleFor(x => x.VisitId)
            .GreaterThan(0)
            .WithMessage("VisitId must be a valid positive integer.");

        RuleFor(x => x.AudioFile)
            .NotNull()
            .WithMessage("Audio file is required.");

        // File size
        RuleFor(x => x.AudioFile.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .When(x => x.AudioFile != null)
            .WithMessage($"Audio file must not exceed {MaxFileSizeBytes / 1_048_576} MB.");

        // Extension check
        RuleFor(x => x.AudioFile.FileName)
            .Must(HaveAllowedExtension)
            .When(x => x.AudioFile != null)
            .WithMessage("Only .wav, .mp3, and .m4a files are accepted.");

        // MIME type check
        RuleFor(x => x.AudioFile.ContentType)
            .Must(HaveAllowedContentType)
            .When(x => x.AudioFile != null)
            .WithMessage("Invalid audio content type. Accepted: wav, mp3, m4a.");
    }

    private static bool HaveAllowedExtension(string FileName)
    {
        var Extension = Path.GetExtension(FileName);
        return !string.IsNullOrEmpty(Extension) && AllowedExtensions.Contains(Extension);
    }

    private static bool HaveAllowedContentType(string ContentType)
        => AllowedContentTypes.Contains(ContentType);
}