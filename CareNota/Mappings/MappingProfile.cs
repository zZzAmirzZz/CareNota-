using AutoMapper;
using CareNota.DTOs;
using CareNota.DTOs.Appointment;
using CareNota.DTOs.Auth;
using CareNota.DTOs.Diagnosis;
using CareNota.DTOs.Doctor;
using CareNota.DTOs.LabTest;
using CareNota.DTOs.Medication;
using CareNota.DTOs.Patient;
using CareNota.DTOs.Prescription;
using CareNota.DTOs.Visit;
using CareNota.Models;
using static CareNota.Models.Appointment;

namespace CareNota.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Patient ─────────────────────────────────────────
        CreateMap<Patient, PatientDto>()
            .ForMember(D => D.FullName,
                O => O.MapFrom(S => S.User.FullName))
            .ForMember(D => D.Email,
                O => O.MapFrom(S => S.User.Email))
            .ForMember(D => D.PhoneNumber,
                O => O.MapFrom(S => S.User.PhoneNumber))
            .ForMember(D => D.Age,
                O => O.MapFrom(S => CalculateAge(S.DateOfBirth)));

        CreateMap<Patient, PatientDetailDto>()
            .IncludeBase<Patient, PatientDto>();

        CreateMap<MedicalHistory, MedicalHistorySummaryDto>();
        CreateMap<Appointment, AppointmentSummaryDto>();

        CreateMap<UpdatePatientDto, Patient>()
            .ForMember(D => D.PatientID, O => O.Ignore())
            .ForMember(D => D.UserId, O => O.Ignore())
            .ForMember(D => D.User, O => O.Ignore())
            .ForMember(D => D.DateOfBirth, O => O.Ignore())
            .ForMember(D => D.MedicalHistory, O => O.Ignore())
            .ForMember(D => D.Appointments, O => O.Ignore())
            .ForMember(D => D.Reminders, O => O.Ignore());

        // ── Doctor ─────────────────────────────────────────
        CreateMap<Doctor, DoctorDto>()
            .ForMember(D => D.FullName,
                O => O.MapFrom(S => S.User.FullName))
            .ForMember(D => D.Email,
                O => O.MapFrom(S => S.User.Email))
            .ForMember(D => D.PhoneNumber,
                O => O.MapFrom(S => S.User.PhoneNumber));

        CreateMap<UpdateDoctorDto, Doctor>()
            .ForMember(D => D.DoctorID, O => O.Ignore())
            .ForMember(D => D.UserId, O => O.Ignore())
            .ForMember(D => D.User, O => O.Ignore());
        // ── Appointment ─────────────────────────────────────

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(src =>
                src.Patient != null ? src.Patient.User.FullName : string.Empty))

            //  DoctorName
            .ForMember(d => d.DoctorName, opt => opt.MapFrom(src =>
                src.Doctor != null ? src.Doctor.User.FullName : string.Empty));


        // Detail
        CreateMap<Appointment, AppointmentDetailDto>()
            .IncludeBase<Appointment, AppointmentDto>();


        // Visit
        CreateMap<Visit, VisitSummaryDto>();


        // ── Create ─────────────────────────────────────────

     

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(d => d.Status, opt => opt.MapFrom(_ => AppointmentStatus.Scheduled))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))

            //  navigation
            .ForMember(d => d.Patient, opt => opt.Ignore())
            .ForMember(d => d.Receptionist, opt => opt.Ignore())
            .ForMember(d => d.Doctor, opt => opt.Ignore()) // 🔥 NEW
            .ForMember(d => d.Visit, opt => opt.Ignore())
            .ForMember(d => d.Reminders, opt => opt.Ignore());


        // ── Update ─────────────────────────────────────────

        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForMember(d => d.AppointmentID, opt => opt.Ignore())
            .ForMember(d => d.PatientID, opt => opt.Ignore())
            .ForMember(d => d.ReceptionistID, opt => opt.Ignore())
            .ForMember(d => d.DoctorID, opt => opt.Ignore()) // 🔥 مهم جدًا

            .ForMember(d => d.CreatedAt, opt => opt.Ignore())

            // navigation
            .ForMember(d => d.Patient, opt => opt.Ignore())
            .ForMember(d => d.Receptionist, opt => opt.Ignore())
            .ForMember(d => d.Doctor, opt => opt.Ignore()) // 🔥 NEW
            .ForMember(d => d.Visit, opt => opt.Ignore())
            .ForMember(d => d.Reminders, opt => opt.Ignore());

        // ── Visit ───────────────────────────────────────────
        CreateMap<Visit, VisitDto>()
            .ForMember(D => D.PatientName, O => O.MapFrom(S =>
                S.Appointment != null && S.Appointment.Patient != null
                    ? S.Appointment.Patient.User.FullName
                    : string.Empty));

        CreateMap<Visit, VisitDetailDto>()
            .IncludeBase<Visit, VisitDto>()
            .ForMember(D => D.Diagnoses,
                O => O.MapFrom(S => S.VisitDiagnoses
                    .Where(VD => VD.Diagnosis != null)
                    .Select(VD => VD.Diagnosis)))
            .ForMember(D => D.Prescription,
                O => O.MapFrom(S => S.Prescription))
            .ForMember(D => D.LabTests,
                O => O.MapFrom(S => S.LabTests))
            .ForMember(D => D.AISummaries,
                O => O.MapFrom(S => S.AISummaries));

        CreateMap<AISummary, AISummarySummaryDto>();

        CreateMap<CreateVisitDto, Visit>()
            .ForMember(D => D.Appointment, O => O.Ignore())
            .ForMember(D => D.Prescription, O => O.Ignore())
            .ForMember(D => D.AudioRecord, O => O.Ignore())
            .ForMember(D => D.LabTests, O => O.Ignore())
            .ForMember(D => D.AISummaries, O => O.Ignore())
            .ForMember(D => D.VisitDiagnoses, O => O.Ignore());

        CreateMap<UpdateVisitDto, Visit>()
            .ForMember(D => D.VisitID, O => O.Ignore())
            .ForMember(D => D.VisitDate, O => O.Ignore())
            .ForMember(D => D.AppointmentID, O => O.Ignore())
            .ForMember(D => D.Appointment, O => O.Ignore())
            .ForMember(D => D.Prescription, O => O.Ignore())
            .ForMember(D => D.AudioRecord, O => O.Ignore())
            .ForMember(D => D.LabTests, O => O.Ignore())
            .ForMember(D => D.AISummaries, O => O.Ignore())
            .ForMember(D => D.VisitDiagnoses, O => O.Ignore());

        // ── Diagnosis ───────────────────────────────────────
        CreateMap<Diagnosis, DiagnosisDto>();
        CreateMap<Diagnosis, DiagnosisSummaryDto>();

        CreateMap<CreateDiagnosisDto, Diagnosis>()
            .ForMember(D => D.VisitDiagnoses, O => O.Ignore());

        // ── Prescription ────────────────────────────────────
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(D => D.PatientName, O => O.MapFrom(S =>
                S.Visit != null &&
               S.Visit != null &&
S.Visit.Appointment != null &&
S.Visit.Appointment.Patient != null
                    ? S.Visit.Appointment.Patient.User.FullName
                    : string.Empty))
            .ForMember(D => D.Medications,
                O => O.MapFrom(S => S.PrescriptionMedications));

        CreateMap<Prescription, PrescriptionSummaryDto>()
            .ForMember(D => D.Medications,
                O => O.MapFrom(S => S.PrescriptionMedications));

        CreateMap<CreatePrescriptionDto, Prescription>()
            .ForMember(D => D.Visit, O => O.Ignore())
            .ForMember(D => D.PrescriptionMedications, O => O.Ignore())
            .ForMember(D => D.Reminders, O => O.Ignore());

        CreateMap<UpdatePrescriptionDto, Prescription>()
            .ForMember(D => D.PrescriptionID, O => O.Ignore())
            .ForMember(D => D.VisitID, O => O.Ignore())
            .ForMember(D => D.Visit, O => O.Ignore())
            .ForMember(D => D.PrescriptionMedications, O => O.Ignore())
            .ForMember(D => D.Reminders, O => O.Ignore());

        // ── PrescriptionMedication ─────────────────────────
        CreateMap<PrescriptionMedication, PrescriptionMedicationDetailDto>()
            .ForMember(D => D.MedicationName,
                O => O.MapFrom(S => S.Medication.MedicationName))
            .ForMember(D => D.MedicationType,
                O => O.MapFrom(S => S.Medication.MedicationType))
            .ForMember(D => D.Strength,
                O => O.MapFrom(S => S.Medication.Strength));

        CreateMap<PrescriptionMedication, PrescriptionMedicationSummaryDto>()
            .ForMember(D => D.MedicationName,
                O => O.MapFrom(S => S.Medication.MedicationName));

        CreateMap<AddMedicationToPrescriptionDto, PrescriptionMedication>()
            .ForMember(D => D.PrescriptionID, O => O.Ignore())
            .ForMember(D => D.Prescription, O => O.Ignore())
            .ForMember(D => D.Medication, O => O.Ignore());

        // ── Medication ─────────────────────────────────────
        CreateMap<Medication, MedicationDto>();

        CreateMap<CreateMedicationDto, Medication>()
            .ForMember(D => D.MedicationID, O => O.Ignore())
            .ForMember(D => D.PrescriptionMedications, O => O.Ignore());

        CreateMap<UpdateMedicationDto, Medication>()
            .ForMember(D => D.MedicationID, O => O.Ignore())
            .ForMember(D => D.MedicationName, O => O.Ignore())
            .ForMember(D => D.PrescriptionMedications, O => O.Ignore());

        // ── LabTest ────────────────────────────────────────
        CreateMap<LabTest, LabTestDto>()
            .ForMember(D => D.PatientName, O => O.MapFrom(S =>
               S.Visit != null &&
S.Visit.Appointment != null &&
S.Visit.Appointment.Patient != null
                    ? S.Visit.Appointment.Patient.User.FullName
                    : string.Empty));

        CreateMap<LabTest, LabTestSummaryDto>();

        CreateMap<CreateLabTestDto, LabTest>()
            .ForMember(D => D.LabTestID, O => O.Ignore())
            .ForMember(D => D.TestResultURL, O => O.MapFrom(_ => string.Empty))
            .ForMember(D => D.Visit, O => O.Ignore());
    }

    private static int CalculateAge(DateTime DateOfBirth)
    {
        var Today = DateTime.Today;
        var Age = Today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > Today.AddYears(-Age)) Age--;
        return Age;
    }
}