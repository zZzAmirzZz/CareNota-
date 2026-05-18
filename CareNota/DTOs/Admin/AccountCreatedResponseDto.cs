namespace CareNota.DTOs.Admin
{
    public class AccountCreatedResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }
}
