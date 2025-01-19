using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NeatPath.Dto.Request;
using NeatPath.Dto.Response;
using NeatPath.Helpers;
using NeatPath.Interfaces;
using NeatPath.Models;
using NeatPath.Models.Enums;

namespace NeatPath.Controllers
{
    [Route("api/v1/[controller]")]
    [Controller]
    public class AuthController(IUserRepository userRepository, ISessionRepository sessionRepository, IPasswordService passwordService, ITokenGenerator tokenGenerator, IMapper mapper) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISessionRepository _sessionRepository = sessionRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(SessionResponseDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(400)]
        public IActionResult Login([FromBody] UserLoginDto userDto)
        {
            if (userDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userRepository.GetUserByUsername(userDto.Username);
            
            if(user == null)
                return Unauthorized("Invalid username");

            if(!_passwordService.VerifyPassword(userDto.Password, user.PasswordHash))
                return Unauthorized("Invalid password");

            var token = _tokenGenerator.GenerateToken();
            var sessionToCreate = _mapper.Map<Session>(new SessionCreateDto()
            {
                Token = token,
                UserId = user.Id,
                // default expiration time - 1h
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
            sessionToCreate.User = user;

            var sessionCreated = _sessionRepository.CreateSession(sessionToCreate);

            if (!sessionCreated)
            {
                ModelState.AddModelError("", "Cannot create session");
                return StatusCode(500, ModelState);
            }

            var session = _sessionRepository.GetSessionByToken(token);

            if(session == null)
            {
                ModelState.AddModelError("", "Something went wrong while retrieving data about session");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<SessionResponseDto>(session));
        }

        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(SessionResponseDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public IActionResult Register([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_userRepository.GetUserByUsername(userDto.Username) != null)
                return Conflict("Username already exists");

            var user = _mapper.Map<User>(userDto);

            if (!_userRepository.CreateUser(user))
            {
                ModelState.AddModelError("", $"Something went wrong while saving user with username: {user.Username}");
            }

            var token = _tokenGenerator.GenerateToken();
            var sessionToCreate = _mapper.Map<Session>(new SessionCreateDto()
            {
                Token = token,
                UserId = user.Id,
                // default expiration time - 1h
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
            sessionToCreate.User = user;

            var sessionCreated = _sessionRepository.CreateSession(sessionToCreate);

            if (!sessionCreated)
            {
                ModelState.AddModelError("", "Cannot create session");
                return StatusCode(500, ModelState);
            }

            var session = _sessionRepository.GetSessionByToken(token);

            if (session == null)
            {
                ModelState.AddModelError("", "Something went wrong while retrieving data about session");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<SessionResponseDto>(session));
        }

        [HttpPost("logout")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult Logout([FromBody] SessionTokenDto sessionDto)
        {
            if (sessionDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            
            var sessionToDel = _sessionRepository.GetSessionByToken(sessionDto.Token);
            if (sessionToDel == null)
                return NotFound();

            if (!_sessionRepository.DeleteSession(sessionToDel))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting session with id: {sessionToDel.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
