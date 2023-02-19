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

        public ApiController(IUrlService urlService)
        {
            _urlService = urlService;
        }
        
        [HttpGet("shorten/")]
        public ActionResult<string> Shorten([FromQuery]string url)
        {
            if (url.Contains("https://localhost:"))
            {
                return BadRequest();
            }
            return Ok($"{HttpContext.GetFullPath()}{_urlService.ApiGetShortenedUrl(url)}");
        }    
        [HttpGet("original/")]
        public string GetOriginalUrl([FromQuery]string url)
        {
            return _urlService.GetOriginalUrl(url);
        }
    }
}
