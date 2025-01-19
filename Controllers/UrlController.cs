using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NeatPath.Dto.Request;
using NeatPath.Dto.Response;
using NeatPath.Interfaces;
using NeatPath.Models;
using NeatPath.Repository;

namespace NeatPath.Controllers
{
    [Route("api/v1/[controller]")]
    [Controller]
    public class UrlController(IUrlRepository urlRepository, IUserRepository userRepository, IUrlService urlService, IConfiguration configuration, IMapper mapper) : Controller
    {
        private readonly IUrlRepository _urlRepository = urlRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUrlService _urlService = urlService;
        private readonly IMapper _mapper = mapper;
        private readonly string? _redirectUrl = configuration["Urls:RedirectUrl"];

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UrlResponseDto>))]
        public IActionResult GetUrls()
        {
            var urls = _mapper.Map<List<UrlResponseDto>>(_urlRepository.GetUrls());
            return Ok(urls);
        }

        [HttpGet("{urlId}")]
        [ProducesResponseType(200, Type = typeof(UrlResponseDto))]
        [ProducesResponseType(400)]
        public IActionResult GetUrl(int urlid)
        {
            if (!_urlRepository.UrlExists(urlid))
                return NotFound();

            var url = _mapper.Map<UrlResponseDto>(_urlRepository.GetUrl(urlid));
            return Ok(url);
        }

        [HttpGet("hash/{hash}")]
        [ProducesResponseType(200, Type = typeof(UrlResponseDto))]
        [ProducesResponseType(400)]
        public IActionResult GetUrlByHash(string hash)
        {
            var url = _urlRepository.GetUrlByHash(hash);
            
            if (url == null)
                return NotFound();

            var urlMapped = _mapper.Map<UrlResponseDto>(url);

            return Ok(urlMapped);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateUrl([FromBody] UrlCreateDto urlCreateDto)
        {
            if (urlCreateDto == null || !ModelState.IsValid) 
                return BadRequest(ModelState);

            var userId = urlCreateDto.UserId;
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _userRepository.GetUser(userId);
            if (user.Role == Models.Enums.UserRole.Anonymous)
            {
                ModelState.AddModelError("", "Anonymous user cannot create urls");
                return StatusCode(422, ModelState);
            }

            if (_urlRepository.GetUrlByOriginalUrl(urlCreateDto.OriginalUrl) != null){
                ModelState.AddModelError("", $"Cannot shorten {urlCreateDto.OriginalUrl} twice.");
                return StatusCode(422, ModelState);
            }

            // get hash, if collision, try 2 times more. if 3 times error, returns error
            string hash;
            int hashAttempt = 0;
            do
            {
                hash = _urlService.HashUrl(urlCreateDto.OriginalUrl);
                ++hashAttempt;
            } while (hashAttempt < 3 && _urlRepository.GetUrlByHash(hash) != null);

            if(hashAttempt == 3 && _urlRepository.GetUrlByHash(hash) != null)
            {
                ModelState.AddModelError("", $"Cannot hash {urlCreateDto.OriginalUrl}");
                return StatusCode(500, ModelState);
            }

            // if cannot get serviceUrl from env variables
            if (String.IsNullOrEmpty(_redirectUrl))
            {
                ModelState.AddModelError("", $"Cannot get serviceUrl");
                return StatusCode(500, ModelState);
            }

            var urlMapped = _mapper.Map<Url>(urlCreateDto);
            urlMapped.Hash = hash;
            urlMapped.ShortUrl = _redirectUrl + hash;
            urlMapped.User = _userRepository.GetUser(userId);

            if (!_urlRepository.CreateUrl(urlMapped))
            {
                ModelState.AddModelError("", $"Something went wrong while creating short url for {urlCreateDto.OriginalUrl}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<UrlResponseDto>(urlMapped));
        }

        [HttpPut("{urlId}/add-click")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult AddClicks(int urlId, [FromBody] UrlClicksDto urlClicksDto)
        {
            if (urlClicksDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_urlRepository.UrlExists(urlId))
                return NotFound();

            var urlToUpd = _urlRepository.GetUrl(urlId);
            urlToUpd.ClickCount += urlClicksDto.ClickCount;

            if (!_urlRepository.UpdateUrl(urlToUpd))
            {
                ModelState.AddModelError("", $"Something went wrong while updating url with id {urlId}");
                return StatusCode(500, ModelState);
            }

            return Ok();
        }

        [HttpDelete("{urlId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUrl(int urlId)
        {
            if (!_urlRepository.UrlExists(urlId)) 
                return NotFound();

            var urlToDel = _urlRepository.GetUrl(urlId);

            if (!_urlRepository.DeleteUrl(urlToDel))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting Url with id {urlId}");
                return StatusCode(500, ModelState );
            }

            return NoContent();
        }
    }
}
