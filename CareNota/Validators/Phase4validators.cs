
using CareNota.DTOs.Audio;
using FluentValidation;
 
namespace CareNota.Validators;

// ══════════════════════════════════════════════════════════════════════════════
// AudioUploadValidator
// ══════════════════════════════════════════════════════════════════════════════
public class AudioUploadValidator : AbstractValidator<AudioUploadDto>
{
    private static readonly string[] AllowedExtensions = [".wav", ".mp3", ".m4a"];
    private static readonly string[] AllowedMimeTypes =
        ["audio/wav", "audio/mpeg", "audio/mp4", "audio/x-m4a", "audio/m4a"];

    private const long MaxBytes = 50 * 1024 * 1024; // 50 MB

    public AudioUploadValidator()
    {
        RuleFor(X => X.AudioFile)
            .NotNull()
            .WithMessage("Audio file is required.")

            .Must(F => F is not null && F.Length > 0)
            .WithMessage("Audio file must not be empty.")

            .Must(F => F is not null && F.Length <= MaxBytes)
            .WithMessage("Audio file must not exceed 50 MB.")

            .Must(F => F is not null &&
                AllowedExtensions.Contains(
                    Path.GetExtension(F.FileName).ToLowerInvariant()))
            .WithMessage("Only .wav, .mp3, and .m4a files are allowed.")

            .Must(F => F is not null &&
                AllowedMimeTypes.Contains(F.ContentType.ToLowerInvariant()))
            .WithMessage("Invalid audio MIME type.");
    }
}
