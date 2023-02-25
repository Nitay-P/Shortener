using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Extensions;
using WebApplication1.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        // GET: api/<ApiController>
        private readonly IUrlService _urlService;
        private readonly IUserService _userService;

        public ApiController(IUrlService urlService,IUserService userService)
        {
            _urlService = urlService;
            _userService = userService;
        }
        
        [HttpGet("shorten/")]
        public async Task <ActionResult<string>> Shorten([FromQuery]string url, [FromQuery] string email)
        {
            if (url.Contains("https://localhost:"))
            {
                return BadRequest();            
            }
            if (await _userService.ApiCheckLogin(email) == false )
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return Ok($"{HttpContext.GetFullPath()}{_urlService.GetShortenedUrl(url,email)}");
        }    
        [HttpGet("original/")]
        public string GetOriginalUrl([FromQuery]string url)
        {
            return _urlService.GetOriginalUrl(url);
        }
    }
}
