using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class UserController : BaseCrudController<User, UserController, UserDto, CreateUserDto>
    {
        public UserController(IUserRepo repo) : base(repo)
        {

        }

        protected override UserDto MapToResponseDto(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone
            };
        }

        protected override User MapToEntity(CreateUserDto createDto)
        {
            return new User
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Phone = createDto.Phone
            };
        }

    }
}
