namespace LostFoundPetReporter.API.DTO
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }

        public UserDto User { get; set; } = null!;
    }
}
