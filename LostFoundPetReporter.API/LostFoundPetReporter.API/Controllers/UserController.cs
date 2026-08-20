using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class UserController : BaseCrudController<User, UserController, UserDto, CreateUserDto>
    {
        public UserController(IUserRepo repo) : base(repo)
        {

        }

        [ApiVersion("1.0")]
        [HttpPost("Login")]
        public ActionResult<UserDto> Login(LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = ((IUserRepo)MainRepo).GetByEmail(loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            if (user.HashedPassword != loginDto.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(UserDto.FromEntity(user));
        }


    }
}
