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
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult LoginUser([FromBody] UserLoginDto userDto)
        {
            if (userDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userRepository.GetUserByUsername(userDto.Username);
            
            if(user == null)
                return Unauthorized("Invalid username");

            if(!_passwordService.VerifyPassword(userDto.Password, user.PasswordHash))
                return Unauthorized("Invalid password");

            var token = _tokenGenerator.GenerateToken();

            var sessionCreated = _sessionRepository.CreateSession(_mapper.Map<Session>(new SessionCreateDto()
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            }));

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

            return Ok(session);
        }

        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult RegisterUser([FromBody] UserCreateDto userDto)
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

            var sessionCreated = _sessionRepository.CreateSession(_mapper.Map<Session>(new SessionCreateDto()
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            }));

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

            return Ok(session);
        }
    }
}
