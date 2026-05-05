namespace CareNota.Services.Interfaces;

public interface IReminderService
{
    Task SendAppointmentConfirmationAsync(int appointmentId);
    Task SendAppointmentCancellationAsync(int appointmentId);
    Task CheckMissedAppointmentsAsync();
    Task SendUpcomingAppointmentRemindersAsync();
    Task SendMedicationRemindersAsync();
}