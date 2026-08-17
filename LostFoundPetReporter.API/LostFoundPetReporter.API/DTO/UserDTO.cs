using LostFoundPetReporter.API.DTO.Interfaces;
namespace LostFoundPetReporter.API.DTO
{
    

    public class UserDto : BaseResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class CreateUserDto : BaseCreateOrUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
