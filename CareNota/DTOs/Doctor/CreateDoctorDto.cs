public class CreateDoctorDto
{
    public string DoctorName { get; set; }
    public string Specialty { get; set; }
    public string UserId { get; set; }  // links to ApplicationUser
}