using CareNota.Data;
using CareNota.Models;
using CareNota.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Services;

public class ReminderService : IReminderService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ReminderService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // ── 1. CONFIRMATION (fires on appointment creation) ───────────────────

    public async Task SendAppointmentConfirmationAsync(int appointmentId)
    {
        var appointment = await GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null) return;

        var email = appointment.Patient.User.Email!;
        var name = appointment.Patient.User.FullName;

        await _emailService.SendAsync(
            email, name,
            "Your Appointment is Confirmed – CareNota",
            EmailTemplates.AppointmentConfirmation(
                name,
                appointment.Doctor.User.FullName,
                appointment.StartTime,
                appointment.AppointmentType));

        _context.Reminders.Add(new Reminder
        {
            PatientID = appointment.PatientID,
            AppointmentID = appointment.AppointmentID,
            ReminderType = "AppointmentConfirmation",
            Message = $"Confirmation email sent for appointment on {appointment.StartTime:f}",
            ReminderDateTime = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    // ── 2. CANCELLATION (fires when CancelAsync is called) ────────────────

    public async Task SendAppointmentCancellationAsync(int appointmentId)
    {
        var appointment = await GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null) return;

        var email = appointment.Patient.User.Email!;
        var name = appointment.Patient.User.FullName;

        await _emailService.SendAsync(
            email, name,
            "Your Appointment Has Been Cancelled – CareNota",
            EmailTemplates.AppointmentCancelled(
                name,
                appointment.Doctor.User.FullName,
                appointment.StartTime));

        _context.Reminders.Add(new Reminder
        {
            PatientID = appointment.PatientID,
            AppointmentID = appointment.AppointmentID,
            ReminderType = "AppointmentCancellation",
            Message = $"Cancellation email sent for appointment on {appointment.StartTime:f}",
            ReminderDateTime = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    // ── 3. MISSED APPOINTMENTS (runs hourly via Hangfire) ─────────────────

    public async Task CheckMissedAppointmentsAsync()
    {
        var now = DateTime.UtcNow;

        var missedAppointments = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a =>
                a.Status == AppointmentStatus.Scheduled &&
                a.EndTime < now)
            .ToListAsync();

        foreach (var appointment in missedAppointments)
        {
            // Mark as Missed via Cancelled (since you have no Missed status)
            // Option B: we mark it Cancelled and note it as missed in the reminder log
            appointment.Status = AppointmentStatus.Cancelled;
            _context.Appointments.Update(appointment);

            var email = appointment.Patient.User.Email!;
            var name = appointment.Patient.User.FullName;

            await _emailService.SendAsync(
                email, name,
                "You Missed Your Appointment – CareNota",
                EmailTemplates.AppointmentMissed(
                    name,
                    appointment.Doctor.User.FullName,
                    appointment.StartTime));

            _context.Reminders.Add(new Reminder
            {
                PatientID = appointment.PatientID,
                AppointmentID = appointment.AppointmentID,
                ReminderType = "AppointmentMissed",
                Message = $"Missed appointment email sent for {appointment.StartTime:f}",
                ReminderDateTime = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    // ── 4. UPCOMING REMINDER (runs hourly via Hangfire) ───────────────────

    public async Task SendUpcomingAppointmentRemindersAsync()
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddHours(23);
        var windowEnd = now.AddHours(25);

        // Only send if we haven't already sent a reminder for this appointment
        var alreadySentIds = await _context.Reminders
            .Where(r => r.ReminderType == "AppointmentUpcoming")
            .Select(r => r.AppointmentID)
            .ToListAsync();

        var upcoming = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a =>
                a.Status == AppointmentStatus.Scheduled &&
                a.StartTime >= windowStart &&
                a.StartTime <= windowEnd &&
                !alreadySentIds.Contains(a.AppointmentID))
            .ToListAsync();

        foreach (var appointment in upcoming)
        {
            var email = appointment.Patient.User.Email!;
            var name = appointment.Patient.User.FullName;

            await _emailService.SendAsync(
                email, name,
                "Reminder: Appointment Tomorrow – CareNota",
                EmailTemplates.AppointmentReminder(
                    name,
                    appointment.Doctor.User.FullName,
                    appointment.StartTime,
                    appointment.AppointmentType));

            _context.Reminders.Add(new Reminder
            {
                PatientID = appointment.PatientID,
                AppointmentID = appointment.AppointmentID,
                ReminderType = "AppointmentUpcoming",
                Message = $"24h reminder sent for appointment on {appointment.StartTime:f}",
                ReminderDateTime = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    // ── 5. MEDICATION REMINDERS (runs hourly via Hangfire) ────────────────

    public async Task SendMedicationRemindersAsync()
    {
        var now = DateTime.UtcNow;

        // Get all active prescriptions with their medications
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Visit)
                .ThenInclude(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(p => p.User)
            .Include(p => p.PrescriptionMedications)
                .ThenInclude(pm => pm.Medication)
            .Where(p => p.Visit != null)
            .ToListAsync();

        foreach (var prescription in prescriptions)
        {
            var patient = prescription.Visit.Appointment.Patient;
            var email = patient.User.Email!;
            var name = patient.User.FullName;

            foreach (var pm in prescription.PrescriptionMedications)
            {
                var intervalHours = ParseFrequencyToHours(pm.Frequency);
                if (intervalHours == null) continue;

                // Check when we last sent this specific medication reminder
                var lastSent = await _context.Reminders
                    .Where(r =>
                        r.PrescriptionID == prescription.PrescriptionID &&
                        r.ReminderType == "Medication" &&
                        r.Message.Contains(pm.Medication.MedicationName))
                    .OrderByDescending(r => r.ReminderDateTime)
                    .Select(r => r.ReminderDateTime)
                    .FirstOrDefaultAsync();

                // If never sent, or enough hours have passed → send
                var hoursSinceLast = (now - lastSent).TotalHours;
                if (lastSent == default || hoursSinceLast >= intervalHours.Value)
                {
                    await _emailService.SendAsync(
                        email, name,
                        $"Medication Reminder: {pm.Medication.MedicationName} – CareNota",
                        EmailTemplates.MedicationReminder(
                            name,
                            pm.Medication.MedicationName,
                            pm.Dosage,
                            pm.Frequency,
                            pm.Route));

                    _context.Reminders.Add(new Reminder
                    {
                        PatientID = patient.PatientID,
                        PrescriptionID = prescription.PrescriptionID,
                        ReminderType = "Medication",
                        Message = $"Medication reminder sent: {pm.Medication.MedicationName}",
                        ReminderDateTime = now
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    // ── HELPERS ───────────────────────────────────────────────────────────

    private async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
    {
        return await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
    }

    private static double? ParseFrequencyToHours(string frequency)
    {
        var f = frequency.ToLower().Trim();

        if (f.Contains("once daily") || f.Contains("once a day") || f == "daily")
            return 24;
        if (f.Contains("twice daily") || f.Contains("twice a day") || f.Contains("2x"))
            return 12;
        if (f.Contains("three times") || f.Contains("3x") || f.Contains("thrice"))
            return 8;
        if (f.Contains("every 4 hour"))
            return 4;
        if (f.Contains("every 6 hour"))
            return 6;
        if (f.Contains("every 8 hour"))
            return 8;
        if (f.Contains("every 12 hour"))
            return 12;
        if (f.Contains("as needed") || f.Contains("prn"))
            return null; // Don't auto-remind for PRN medications

        return null; // Unknown format → skip
    }
}