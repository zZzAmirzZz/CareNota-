public class AppointmentDto
{
    public int AppointmentID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string AppointmentType { get; set; }

    // Return IDs + names for display
    public int PatientID { get; set; }
    public int DoctorID { get; set; }
    public int ReceptionistID { get; set; }
}