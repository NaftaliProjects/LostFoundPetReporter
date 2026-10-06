using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.API.Services.Authentication;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;




namespace LostFoundPetReporter.API.Controllers
{
    public class UserController : BaseCrudController<User, UserDto, CreateUserDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IUserDeviceRepo _userDeviceRepo;

        public UserController(IUserRepo repo, IJwtService jwtService, IUserDeviceRepo userDeviceRepo) : base(repo)
        {
            _jwtService = jwtService;
            _userDeviceRepo = userDeviceRepo;
        }


        [ApiVersion("1.0")]
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<LoginResponseDto> Login(LoginUserDto loginDto)
        {

            var user = ((IUserRepo)_mainRepo).GetByEmail(loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid Email or password.");
            }

            if (user.HashedPassword != loginDto.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _jwtService.CreateToken(
                user.Id,
                user.Email,
                out var expiresAt);

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
            if (createDto.Id.HasValue && createDto.Id.Value > 0) { return BadRequest("POST requests cannot specify an existing Id."); }

            var entity = createDto.ToEntity();

            try { _mainRepo.Add(entity); }
            catch (Exception ex) { return BadRequest(ex); }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, UserDto.FromEntity(entity));
        }


        [ApiVersion("1.0")]
        [Authorize]
        [HttpPost("RegisterDevice")]
        public ActionResult RegisterDevice( RegisterDeviceTokenDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                return BadRequest("FCM token is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Platform))
            {
                return BadRequest("Platform is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }


            var createDto = new CreateUserDeviceDto
            {
                UserId = userId.Value,
                Token = dto.Token,
                Platform = dto.Platform,
                LastUpdated = DateTime.UtcNow
            };

            var entity = createDto.ToEntity();

            _userDeviceRepo.RegisterDevice(entity);

            return Ok(new
            {
                Message = "Device token registered successfully."
            });
        }


    }
}
