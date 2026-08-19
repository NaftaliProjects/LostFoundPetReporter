using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.API.DTO
{
    public class UserDto : IResponseDto<User, UserDto>, IHasId

    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;


        public static UserDto FromEntity(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone
            };
        }
    }


    public class CreateUserDto : IEntityDto<User>, IHasId
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;


        public User ToEntity()
        {
            return new User
            {
                Id = Id ?? 0,
                Name = Name,
                HashedPassword = Password,
                Email = Email,
                Phone = Phone
            };
        }
    }
}