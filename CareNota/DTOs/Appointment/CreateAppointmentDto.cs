public class CreateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string AppointmentType { get; set; }

    // FKs — frontend must send these
    public int PatientID { get; set; }
    public int ReceptionistID { get; set; }
    public int DoctorID { get; set; }
}