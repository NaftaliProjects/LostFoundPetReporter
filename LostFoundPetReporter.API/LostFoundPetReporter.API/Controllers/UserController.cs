using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.API.Services.Authentication;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.AspNetCore.Authorization;




namespace LostFoundPetReporter.API.Controllers
{
    public class UserController : BaseCrudController<User, UserController, UserDto, CreateUserDto>
    {
        private readonly IJwtService _jwtService;

        public UserController(IUserRepo repo, IJwtService jwtService) : base(repo)
        {
            _jwtService = jwtService;
        }

        [ApiVersion("1.0")]
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<LoginResponseDto> Login(LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = ((IUserRepo)MainRepo).GetByEmail(loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid Email or password.");
            }

            if (user.HashedPassword != loginDto.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _jwtService.CreateToken(user.Id, user.Email, out var expiresAt);


            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = UserDto.FromEntity(user)
            });
        }


        [ApiVersion("1.0")]
        [AllowAnonymous]
        [HttpPost("Register")]
        public virtual ActionResult<UserDto> AddOne(CreateUserDto createDto)
        {
            if (!ModelState.IsValid) { return ValidationProblem(ModelState); }
            if (createDto.Id.HasValue && createDto.Id.Value > 0) { return BadRequest("POST requests cannot specify an existing Id."); }

            var entity = createDto.ToEntity();

            try { MainRepo.Add(entity); }
            catch (Exception ex) { return BadRequest(ex); }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, UserDto.FromEntity(entity));
        }


    }
}
