namespace CareNota.Services;

public static class EmailTemplates
{
    // ── APPOINTMENT CONFIRMATION ──────────────────────────────────────────

    public static string AppointmentConfirmation(
        string patientName,
        string doctorName,
        DateTime startTime,
        string appointmentType)
    {
        return $"""
        <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #e0e0e0;border-radius:8px;">
            <h2 style="color:#2E86AB;">✅ Appointment Confirmed</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>Your appointment has been successfully scheduled. Here are the details:</p>
            <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Doctor</td>
                    <td style="padding:8px;">Dr. {doctorName}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Date</td>
                    <td style="padding:8px;">{startTime:dddd, MMMM dd yyyy}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Time</td>
                    <td style="padding:8px;">{startTime:hh:mm tt}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Type</td>
                    <td style="padding:8px;">{appointmentType}</td></tr>
            </table>
            <p style="color:#888;font-size:12px;">If you need to cancel or reschedule, please contact us as soon as possible.</p>
            <p style="color:#2E86AB;font-weight:bold;">CareNota Clinic</p>
        </div>
        """;
    }

    // ── APPOINTMENT REMINDER (24h before) ────────────────────────────────

    public static string AppointmentReminder(
        string patientName,
        string doctorName,
        DateTime startTime,
        string appointmentType)
    {
        return $"""
        <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #e0e0e0;border-radius:8px;">
            <h2 style="color:#F4A261;">🔔 Appointment Reminder</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>This is a friendly reminder that you have an appointment <strong>tomorrow</strong>:</p>
            <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Doctor</td>
                    <td style="padding:8px;">Dr. {doctorName}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Date</td>
                    <td style="padding:8px;">{startTime:dddd, MMMM dd yyyy}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Time</td>
                    <td style="padding:8px;">{startTime:hh:mm tt}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Type</td>
                    <td style="padding:8px;">{appointmentType}</td></tr>
            </table>
            <p style="color:#888;font-size:12px;">Please arrive 10 minutes early. Contact us if you need to reschedule.</p>
            <p style="color:#2E86AB;font-weight:bold;">CareNota Clinic</p>
        </div>
        """;
    }

    // ── APPOINTMENT CANCELLED ─────────────────────────────────────────────

    public static string AppointmentCancelled(
        string patientName,
        string doctorName,
        DateTime startTime)
    {
        return $"""
        <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #e0e0e0;border-radius:8px;">
            <h2 style="color:#E63946;">❌ Appointment Cancelled</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>Your appointment has been <strong>cancelled</strong>. Details of the cancelled appointment:</p>
            <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Doctor</td>
                    <td style="padding:8px;">Dr. {doctorName}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Date</td>
                    <td style="padding:8px;">{startTime:dddd, MMMM dd yyyy}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Time</td>
                    <td style="padding:8px;">{startTime:hh:mm tt}</td></tr>
            </table>
            <p>Please contact us to reschedule at your earliest convenience.</p>
            <p style="color:#2E86AB;font-weight:bold;">CareNota Clinic</p>
        </div>
        """;
    }

    // ── APPOINTMENT MISSED ────────────────────────────────────────────────

    public static string AppointmentMissed(
        string patientName,
        string doctorName,
        DateTime startTime)
    {
        return $"""
        <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #e0e0e0;border-radius:8px;">
            <h2 style="color:#E63946;">⚠️ Missed Appointment</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>Our records show that you <strong>missed</strong> your appointment:</p>
            <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Doctor</td>
                    <td style="padding:8px;">Dr. {doctorName}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Date</td>
                    <td style="padding:8px;">{startTime:dddd, MMMM dd yyyy}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Time</td>
                    <td style="padding:8px;">{startTime:hh:mm tt}</td></tr>
            </table>
            <p>Please contact us to book a new appointment.</p>
            <p style="color:#2E86AB;font-weight:bold;">CareNota Clinic</p>
        </div>
        """;
    }

    // ── MEDICATION REMINDER ───────────────────────────────────────────────

    public static string MedicationReminder(
        string patientName,
        string medicationName,
        string dosage,
        string frequency,
        string route)
    {
        return $"""
        <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;border:1px solid #e0e0e0;border-radius:8px;">
            <h2 style="color:#2E86AB;">💊 Medication Reminder</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>This is a reminder to take your medication:</p>
            <table style="width:100%;border-collapse:collapse;margin:16px 0;">
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Medication</td>
                    <td style="padding:8px;">{medicationName}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Dosage</td>
                    <td style="padding:8px;">{dosage}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Frequency</td>
                    <td style="padding:8px;">{frequency}</td></tr>
                <tr><td style="padding:8px;background:#f5f5f5;font-weight:bold;">Route</td>
                    <td style="padding:8px;">{route}</td></tr>
            </table>
            <p style="color:#888;font-size:12px;">If you have any concerns about your medication, contact your doctor.</p>
            <p style="color:#2E86AB;font-weight:bold;">CareNota Clinic</p>
        </div>
        """;
    }
}