using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NeatPath.Dto.Request;
using NeatPath.Dto.Response;
using NeatPath.Interfaces;
using NeatPath.Models;

namespace NeatPath.Controllers
{
    [Route("api/v1/[controller]")]
    [Controller]
    public class SessionController(ISessionRepository sessionRepository, IUserRepository userRepository, IMapper mapper) : Controller
    {
        private readonly ISessionRepository _sessionRepository = sessionRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SessionResponseDto>))]
        public IActionResult GetSessions()
        {
            var sessions = _mapper.Map<List<SessionResponseDto>>(_sessionRepository.GetSessions());
            return Ok(sessions);
        }

        [HttpGet("{sessionId}")]
        [ProducesResponseType(200, Type = typeof(SessionResponseDto))]
        public IActionResult GetSession(int sessionId)
        {
            if (!_sessionRepository.SessionExists(sessionId))
                return NotFound();

            var session = _mapper.Map<SessionResponseDto>(_sessionRepository.GetSession(sessionId));
            return Ok(session);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateSession([FromBody] SessionCreateDto sessionCreateDto)
        {
            if (sessionCreateDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = sessionCreateDto.UserId;
            if (!_userRepository.UserExists(userId))
                return NotFound();

            // single session for user?

            var sessionMapped = _mapper.Map<Session>(sessionCreateDto);
            sessionMapped.User = _userRepository.GetUser(userId);

            if (_sessionRepository.CreateSession(sessionMapped))
            {
                ModelState.AddModelError("", $"Something went wrong while creating session for user with Id {userId}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<SessionResponseDto>(sessionMapped));
        }

        [HttpPut("{sessionId}/expiring-date")]
        [ProducesResponseType(200, Type = typeof(SessionResponseDto))]
        [ProducesResponseType(404)]
        public IActionResult UpdateExpringDate(int sessionId, [FromBody] SessionExpiringDateDto sessionUpdateDto)
        {
            if(sessionUpdateDto == null || sessionUpdateDto.ExpiresAt == DateTime.MinValue || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_sessionRepository.SessionExists(sessionId))
                return NotFound();

            var session = _sessionRepository.GetSession(sessionId);
            session.ExpiresAt = sessionUpdateDto.ExpiresAt;

            _sessionRepository.UpdateSession(session);

            return Ok(_mapper.Map<SessionResponseDto>(session));
        }

        [HttpDelete]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public IActionResult DeleteSession(int sessionId)
        {
            if (!_sessionRepository.SessionExists(sessionId))
                return NotFound();

            var sessionToDel = _sessionRepository.GetSession(sessionId);

            if (!_sessionRepository.DeleteSession(sessionToDel))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting session with id: {sessionId}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
