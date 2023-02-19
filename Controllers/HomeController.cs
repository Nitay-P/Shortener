using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Extensions;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("/")]
    [Authorize(Policy = "MustBeARegisteredUser")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUrlService _urlService;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUrlService urlService,IUserService userService)
        {
            _logger = logger;
            _urlService = urlService;
            _userService = userService;   
        }

        [HttpGet("")]
        public IActionResult Index(string url)
        {
            return View("Index",url);
        }
        [HttpGet("links")]
        public async Task<IActionResult> Links(User user)
        {
            return View(await _userService.GetUserByEmail(User.GetEmail()));
        }
        [HttpPost,Route("")]
        public IActionResult GetShortenedUrl(string url)
        {
            if (url.Contains($"{HttpContext.GetFullPath()}"))
            {
                return RedirectToAction("Index", "Home", new { url = _urlService.GetOriginalUrl(url)});
            }
            return RedirectToAction("Index", "Home",new { url = $"{HttpContext.GetFullPath()}{_urlService.GetShortenedUrl(url, User.GetEmail())}"});
            }

        
        [HttpGet("s/{url}")]
        public IActionResult a(string url)
        {
            string oldUrl = _urlService.Redirect(url);
            if(string.IsNullOrEmpty(oldUrl))
            {
                return RedirectToAction("Index","Home");
            }
            return Redirect(oldUrl);
        }
        
    }
}