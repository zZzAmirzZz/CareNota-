using AutoMapper;
using CareNota.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient
        CreateMap<Patient, PatientDto>();
        CreateMap<CreatePatientDto, Patient>();

        // Doctor
        CreateMap<Doctor, DoctorDto>();
        CreateMap<CreateDoctorDto, Doctor>();

        // Appointment
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<CreateAppointmentDto, Appointment>();
    }
}