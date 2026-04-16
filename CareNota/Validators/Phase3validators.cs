// Validators/Appointment/CreateAppointmentValidator.cs
using CareNota.DTOs.Appointment;
using CareNota.DTOs.Diagnosis;
using CareNota.DTOs.Doctor;
using CareNota.DTOs.LabTest;
using CareNota.DTOs.Medication;
using CareNota.DTOs.Patient;
using CareNota.DTOs.Prescription;
using CareNota.DTOs.Visit;
using FluentValidation;

namespace CareNota.Validators.Appointment;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientID)
            .GreaterThan(0).WithMessage("PatientID must be a valid positive number.");

        RuleFor(x => x.ReceptionistID)
            .GreaterThan(0).WithMessage("ReceptionistID must be a valid positive number.");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

        RuleFor(x => x.AppointmentType)
            .NotEmpty().WithMessage("Appointment type is required.")
            .MaximumLength(100).WithMessage("Appointment type cannot exceed 100 characters.");
    }
}
// Validators/Appointment/UpdateAppointmentValidator.cs

public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentDto>
{
    private static readonly string[] AllowedStatuses =
        ["Scheduled", "Completed", "Cancelled"];

    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.AppointmentDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.")
            .When(x => x.AppointmentDate != default);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => AllowedStatuses.Contains(s))
            .WithMessage("Status must be Scheduled, Completed, or Cancelled.");

        RuleFor(x => x.AppointmentType)
            .NotEmpty().WithMessage("Appointment type is required.")
            .MaximumLength(100).WithMessage("Appointment type cannot exceed 100 characters.");
    }
}
// Validators/Diagnosis/CreateDiagnosisValidator.cs
public class CreateDiagnosisValidator : AbstractValidator<CreateDiagnosisDto>
{
    public CreateDiagnosisValidator()
    {
        RuleFor(x => x.ICD10Code)
            .NotEmpty().WithMessage("ICD10 code is required.")
            .MaximumLength(10).WithMessage("ICD10 code cannot exceed 10 characters.")
            .Matches(@"^[A-Z][0-9]{2}(\.[0-9A-Z]{1,4})?$")
            .WithMessage("ICD10 code format is invalid (e.g. A00 or A00.1).");

        RuleFor(x => x.DiagnosisName)
            .NotEmpty().WithMessage("Diagnosis name is required.")
            .MaximumLength(200).WithMessage("Diagnosis name cannot exceed 200 characters.");
    }
}
// Validators/Diagnosis/AssignDiagnosisToVisitValidator.cs


public class AssignDiagnosisToVisitValidator : AbstractValidator<AssignDiagnosisToVisitDto>
{
    public AssignDiagnosisToVisitValidator()
    {
        RuleFor(x => x.ICD10Code)
            .NotEmpty().WithMessage("ICD10 code is required.")
            .MaximumLength(10).WithMessage("ICD10 code cannot exceed 10 characters.")
            .Matches(@"^[A-Z][0-9]{2}(\.[0-9A-Z]{1,4})?$")
            .WithMessage("ICD10 code format is invalid (e.g. A00 or A00.1).");
    }
}
// Validators/Doctor/UpdateDoctorValidator.cs

public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorDto>
{
    public UpdateDoctorValidator()
    {
        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("Specialty is required.")
            .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters.");
    }
}
// Validators/LabTest/CreateLabTestValidator.cs


public class CreateLabTestValidator : AbstractValidator<CreateLabTestDto>
{
    public CreateLabTestValidator()
    {
        RuleFor(x => x.LabTestName)
            .NotEmpty().WithMessage("Lab test name is required.")
            .MaximumLength(150).WithMessage("Lab test name cannot exceed 150 characters.");

        RuleFor(x => x.VisitID)
            .GreaterThan(0).WithMessage("VisitID must be a valid positive number.");
    }
}
// Validators/LabTest/UploadLabResultValidator.cs


public class UploadLabResultValidator : AbstractValidator<UploadLabResultDto>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public UploadLabResultValidator()
    {
        RuleFor(x => x.ResultFile)
            .NotNull().WithMessage("Result file is required.")
            .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
            .Must(f => f.Length <= MaxFileSizeBytes).WithMessage("File size cannot exceed 5MB.")
            .Must(f => AllowedExtensions.Contains(
                Path.GetExtension(f.FileName).ToLower()))
            .WithMessage("Only PDF, JPG, JPEG, and PNG files are allowed.");
    }
}
// Validators/Medication/CreateMedicationValidator.cs


public class CreateMedicationValidator : AbstractValidator<CreateMedicationDto>
{
    public CreateMedicationValidator()
    {
        RuleFor(x => x.MedicationName)
            .NotEmpty().WithMessage("Medication name is required.")
            .MaximumLength(150).WithMessage("Medication name cannot exceed 150 characters.");

        RuleFor(x => x.MedicationType)
            .NotEmpty().WithMessage("Medication type is required.")
            .MaximumLength(100).WithMessage("Medication type cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Strength)
            .NotEmpty().WithMessage("Strength is required.")
            .MaximumLength(50).WithMessage("Strength cannot exceed 50 characters.");
    }
}
// Validators/Medication/UpdateMedicationValidator.cs


public class UpdateMedicationValidator : AbstractValidator<UpdateMedicationDto>
{
    public UpdateMedicationValidator()
    {
        RuleFor(x => x.MedicationType)
            .NotEmpty().WithMessage("Medication type is required.")
            .MaximumLength(100).WithMessage("Medication type cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Strength)
            .NotEmpty().WithMessage("Strength is required.")
            .MaximumLength(50).WithMessage("Strength cannot exceed 50 characters.");
    }
}
// Validators/Patient/UpdatePatientValidator.cs


public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    private static readonly string[] AllowedGenders = ["Male", "Female", "Other"];
    private static readonly string[] AllowedBloodTypes =
        ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];

    public UpdatePatientValidator()
    {
        RuleFor(x => x.Gender)
            .Must(g => AllowedGenders.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other.")
            .When(x => !string.IsNullOrEmpty(x.Gender));

        RuleFor(x => x.BloodType)
            .Must(b => AllowedBloodTypes.Contains(b))
            .WithMessage("Invalid blood type. Must be A+, A-, B+, B-, AB+, AB-, O+, or O-.")
            .When(x => !string.IsNullOrEmpty(x.BloodType));

        RuleFor(x => x.Allergies)
            .MaximumLength(500).WithMessage("Allergies cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Allergies));

        RuleFor(x => x.InsuranceInfo)
            .MaximumLength(300).WithMessage("Insurance info cannot exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.InsuranceInfo));
    }
}
// Validators/Prescription/CreatePrescriptionValidator.cs


public class CreatePrescriptionValidator : AbstractValidator<CreatePrescriptionDto>
{
    public CreatePrescriptionValidator()
    {
        RuleFor(x => x.VisitID)
            .GreaterThan(0).WithMessage("VisitID must be a valid positive number.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required.")
            .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");
    }
}
// Validators/Prescription/UpdatePrescriptionValidator.cs


public class UpdatePrescriptionValidator : AbstractValidator<UpdatePrescriptionDto>
{
    public UpdatePrescriptionValidator()
    {
        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required.")
            .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");
    }
}
// Validators/Prescription/AddMedicationToPrescriptionValidator.cs


public class AddMedicationToPrescriptionValidator : AbstractValidator<AddMedicationToPrescriptionDto>
{
    public AddMedicationToPrescriptionValidator()
    {
        RuleFor(x => x.MedicationID)
            .GreaterThan(0).WithMessage("MedicationID must be a valid positive number.");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required.")
            .MaximumLength(100).WithMessage("Dosage cannot exceed 100 characters.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required.")
            .MaximumLength(100).WithMessage("Frequency cannot exceed 100 characters.");

        RuleFor(x => x.Route)
            .NotEmpty().WithMessage("Route is required.")
            .MaximumLength(100).WithMessage("Route cannot exceed 100 characters.");

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required.")
            .MaximumLength(100).WithMessage("Duration cannot exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
// Validators/Visit/CreateVisitValidator.cs


public class CreateVisitValidator : AbstractValidator<CreateVisitDto>
{
    public CreateVisitValidator()
    {
        RuleFor(x => x.AppointmentID)
            .GreaterThan(0).WithMessage("AppointmentID must be a valid positive number.");

        RuleFor(x => x.VisitDate)
            .NotEmpty().WithMessage("Visit date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Visit date cannot be in the future.");

        RuleFor(x => x.Subjective)
            .MaximumLength(1000).WithMessage("Subjective cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Subjective));

        RuleFor(x => x.Objective)
            .MaximumLength(1000).WithMessage("Objective cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Objective));

        RuleFor(x => x.Assessment)
            .MaximumLength(1000).WithMessage("Assessment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Assessment));

        RuleFor(x => x.Plan)
            .MaximumLength(1000).WithMessage("Plan cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Plan));
    }
}
// Validators/Visit/UpdateVisitValidator.cs


public class UpdateVisitValidator : AbstractValidator<UpdateVisitDto>
{
    public UpdateVisitValidator()
    {
        RuleFor(x => x.Subjective)
            .MaximumLength(1000).WithMessage("Subjective cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Subjective));

        RuleFor(x => x.Objective)
            .MaximumLength(1000).WithMessage("Objective cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Objective));

        RuleFor(x => x.Assessment)
            .MaximumLength(1000).WithMessage("Assessment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Assessment));

        RuleFor(x => x.Plan)
            .MaximumLength(1000).WithMessage("Plan cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Plan));
    }
}
