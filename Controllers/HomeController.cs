
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
        public async Task<IActionResult> Index(MainPageViewModel mainPageViewModel)
        {
            if (User.Identity!.IsAuthenticated)
            {
                mainPageViewModel.User = await _userService.GetUserByEmail(User.GetEmail());
            }
            return View("Index",mainPageViewModel);
        }
        [HttpGet("links")]
        public async Task<IActionResult> Links(User user)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return View(await _userService.GetUserByEmail(User.GetEmail()));
            }
            return RedirectToAction("Index");
        }
        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public IActionResult GetShortenedUrl(string url)
        {
            if (url.Contains($"{HttpContext.GetFullPath()}"))
            {
                return RedirectToAction("Index", "Home", new { url = _urlService.GetOriginalUrl(url)});
            }
            return RedirectToAction("Index", "Home",new MainPageViewModel {Url = $"{HttpContext.GetFullPath()}{_urlService.GetShortenedUrl(url, User.GetEmail())}"});
            }
         
        
        [HttpGet("s/{url}")]
        public async Task< IActionResult> MyRedirect(string url)
        {
            //user friendly: if shortened url doesn't exist I redirect him to home page
            string oldUrl = await _urlService.Redirect(url);
            if(string.IsNullOrEmpty(oldUrl))
            {
               return RedirectToAction("Index","Home");
            }
            return Redirect(oldUrl);            
        }
        
    }
}