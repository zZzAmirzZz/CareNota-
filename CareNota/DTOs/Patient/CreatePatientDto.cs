public class CreatePatientDto
{
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string BloodType { get; set; }
    public string Allergies { get; set; }
    public string InsuranceInfo { get; set; }
    public string UserId { get; set; }  // links to ApplicationUser
}