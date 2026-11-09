using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.API.Services.Authentication;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        private string CurrentUsername => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(UserRequest req, CancellationToken ct)
        {
            if ( string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(req.UserName))
                return BadRequest(new ProblemDetails { Title = "Password and UserName is required.", Status = StatusCodes.Status404NotFound });

            var newUserResult = await _users.RegisterAsync(req, ct);                

            if (newUserResult.Error == UserError.EmailTaken)
                return Conflict(new ProblemDetails { Title = "Email already registered" });

            return CreatedAtAction(nameof(GetCurrentUser), newUserResult.Value);

        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult> Login(UserLoginRequest req, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(req.UserName))
                return BadRequest("Password and UserName is required.");

            var result = await _users.LoginAsync(req, ct);

            if (result.Error == UserError.NotFound)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            if (result.Error == UserError.InvalidCredentials)
                return Unauthorized(new ProblemDetails { Title = "Invalid credentials", Status = StatusCodes.Status401Unauthorized });

            if (result.Error == UserError.EmailTaken)
                return Conflict(new ProblemDetails { Title = "Email already registered", Status = StatusCodes.Status409Conflict });

            return Ok(new { token = result.Value });
        }

        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken ct)
        {
            var currentUserResult = await _users.GetCurrentUserAsync(CurrentUsername, ct);

            if (currentUserResult.Error == UserError.NotFound)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            return Ok(currentUserResult.Value);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateOne(string username, UserRequest req, CancellationToken ct)
        {
            if (username != CurrentUsername) return Forbid();

            var result = await _users.UpdateAsync(username, req, ct);

            if (result.Error == UserError.NotFound)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            if (result.Error == UserError.EmailTaken)
                return Conflict(new ProblemDetails { Title = "Email already registered", Status = StatusCodes.Status409Conflict });

            return Ok();
        }



        [ApiVersion("1.0")]
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string username, CancellationToken ct)
        {
            return BadRequest(); 
        }


   
        [ApiVersion("1.0")]
        [Authorize]
        [HttpPost("RegisterDevice")]
        public ActionResult RegisterDevice( RegisterDeviceRequest req)
        {

            if (string.IsNullOrEmpty(req.Token))
            {
                return BadRequest(new ProblemDetails { Title = "FCM token is required", Status = StatusCodes.Status400BadRequest });
            }

            if (string.IsNullOrEmpty(req.Platform))
            {
                return BadRequest(new ProblemDetails { Title = "Platform is required", Status = StatusCodes.Status400BadRequest });
            }

            if (CurrentUsername == null)
            {
                return Unauthorized();
            }


            var createDto = new CreateUserDeviceDto
            {
                UserName = CurrentUsername,
                Token = req.Token,
                Platform = req.Platform,
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
