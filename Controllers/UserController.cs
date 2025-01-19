using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NeatPath.Dto.Request;
using NeatPath.Dto.Response;
using NeatPath.Interfaces;
using NeatPath.Models;
using NeatPath.Models.Enums;

namespace NeatPath.Controllers
{
    [Route("api/v1/[controller]")]
    [Controller]
    public class UserController(IUserRepository userRepository, IPasswordService passwordService, IMapper mapper) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserResponseDto>))]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserResponseDto>>(_userRepository.GetUsers());
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(UserResponseDto))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _mapper.Map<UserResponseDto>(_userRepository.GetUser(userId));

            return Ok(user);
        }

        [HttpGet("{userId}/sessions")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SessionResponseDto>))]
        public IActionResult GetUserSessions(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var sessions = _userRepository.GetUserSessions(userId);
            var sessionsMapped = _mapper.Map<List<SessionResponseDto>>(sessions);

            return Ok(sessionsMapped);
        }

        [HttpGet("{userId}/urls")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UrlResponseDto>))]
        public IActionResult GetUserUrls(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var urls = _userRepository.GetUserUrls(userId);

            var urlsMapped = _mapper.Map<List<UrlResponseDto>>(urls);

            return Ok(urlsMapped);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public IActionResult CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDuplicate = _userRepository.GetUsers()
                .Where(u => u.Username == userCreateDto.Username)
                .FirstOrDefault();

            if(userDuplicate != null)
                return Conflict($"User {userCreateDto.Username} is already exists");

            var userMap = _mapper.Map<User>(userCreateDto);
            userMap.PasswordHash = _passwordService.HashPassword(userCreateDto.Password);
            userMap.Role = userCreateDto.Role ?? UserRole.Anonymous;

            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", $"Something went wrong while creating user {userMap.Username}");
                return StatusCode(500, ModelState);
            }
            
            return Created();
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(UserResponseDto))]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if(userUpdateDto.Role == UserRole.Anonymous)
            {
                ModelState.AddModelError("", "Cannot change user role to anonymous since user has been authorized once");
                return StatusCode(400, ModelState);
            }

            if (!_userRepository.UserExists(userId))
                return NotFound();

            var userToUpd = _userRepository.GetUser(userId);

            if(userUpdateDto.Password != null && userToUpd.Role == UserRole.Anonymous && userUpdateDto.Role != UserRole.Anonymous)
            {
                ModelState.AddModelError("", "Cannot apply password for anonymous user or change password within this method");
                return StatusCode(400, ModelState);
            }

            // since user is no longer anonymous, we can apply password 
            if (userUpdateDto.Password != null)
                userToUpd.PasswordHash = _passwordService.HashPassword(userUpdateDto.Password);
            if (userUpdateDto.Username != null)
                userToUpd.Username = userUpdateDto.Username;
            if (userUpdateDto.Role.HasValue)
                userToUpd.Role = userUpdateDto.Role.Value;

            if (!_userRepository.UpdateUser(userToUpd))
            {
                ModelState.AddModelError("", $"Something went wrong while updating user with id: {userId}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<UserResponseDto>(userToUpd));
        }

        [HttpPut("{userId}/change-password")]
        [ProducesResponseType(200, Type = typeof(UserResponseDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult ChangeUserPassword(int userId, [FromBody] UserChangePasswordDto userUpdateDto)
        {
            if (userUpdateDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.UserExists(userId))
                return NotFound();

            var userToUpd = _userRepository.GetUser(userId);

            if(!_passwordService.VerifyPassword(userUpdateDto.CurrentPassword, userToUpd.PasswordHash))
                return Unauthorized("Given password does not match with current password");

            userToUpd.PasswordHash = _passwordService.HashPassword(userUpdateDto.NewPassword);

            if (!_userRepository.UpdateUser(userToUpd))
            {
                ModelState.AddModelError("", $"Something went wrong while updating user with id: {userId}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<UserResponseDto>(userToUpd));
        }

        [HttpPost("{userId}/verify-password")]
        [ProducesResponseType(200, Type = typeof(UserResponseDto))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult VerifyUserPassword(int userId, [FromBody] UserVerifyPasswordDto userDto)
        {
            if (userDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _userRepository.GetUser(userId);
            if (user.PasswordHash == null)
            {
                ModelState.AddModelError("", $"User with id ${userId} does not has password");
                return StatusCode(400, ModelState);
            }
            if (!_passwordService.VerifyPassword(userDto.Password, user.PasswordHash))
                return Unauthorized("Password does not match");
            
            return Ok("Password matches");
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var userToDel = _userRepository.GetUser(userId);

            if (!_userRepository.DeleteUser(userToDel))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting user with id: {userId}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
