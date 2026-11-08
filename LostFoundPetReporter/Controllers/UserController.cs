using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.API.Services.Authentication;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.Services.UserServie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;




namespace LostFoundPetReporter.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;
        public UserController(IUserService users) => _users = users;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest req, CancellationToken ct)
        {
            var result = await _users.RegisterAsync(req, ct);                

            if (result.Error == UserError.EmailTaken)
                return Conflict(new ProblemDetails { Title = "Email already registered" });

            return CreatedAtAction(nameof(GetMe), result.Value);

        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult> Login(LoginRequest req, CancellationToken ct)
        {
            var result = await _users.LoginAsync(req, ct);
            return result.Succeeded ? Ok(new { token = result.Value }) : Unauthorized();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
        {
            var dto = await _users.GetAsync(CurrentUserId, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOne(int id, UpdateUserRequest req, CancellationToken ct)
        {
            if (id != CurrentUserId) return Forbid();

            var result = await _users.UpdateAsync(id, req, ct);
            return result.Error switch
            {
                UserError.None => NoContent(),
                UserError.NotFound => NotFound(),
                UserError.EmailTaken => Conflict(new ProblemDetails { Title = "Email already in use" }),
                _ => Problem()
            };
        }



        // =========================
        // POST
        // =========================

        [ApiVersion("1.0")]
        [HttpPost]
        public virtual ActionResult<TResponseDto> AddOne(TCreateOrUpdateDto createDto)
        {
            if (createDto.Id.HasValue && createDto.Id.Value > 0) { return BadRequest("POST requests cannot specify an existing Id."); }

            var entity = createDto.ToEntity();

            try { _mainRepo.Add(entity); }
            catch (Exception ex) { return BadRequest(ex); }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, TResponseDto.FromEntity(entity));
        }


        // =========================
        // DELETE
        // =========================

        [ApiVersion("1.0")]
        [HttpDelete("{id}")]
        public ActionResult DeleteOne(int id)
        {
            var entity = _mainRepo.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                _mainRepo.Delete(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException()?.Message);

            }

            return NoContent();
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

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
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
            if (createDto.Id.HasValue && createDto.Id.Value > 0) { return BadRequest("POST requests cannot specify an existing Id."); }

            var entity = createDto.ToEntity();

            entity.HashedPassword = _passwordHasher.HashPassword(entity, createDto.Password);


            try
            {
                _mainRepo.Add(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register user.");

                return Problem(
                    title: "Registration failed.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

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
