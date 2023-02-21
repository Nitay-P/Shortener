using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using GoogleAuthentication.Services;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public UserController(IUserService userService,IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        [HttpGet("register")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid || user.Links == null)
            {
                if(user.ConfirmPassword != user.Password)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                    GetGoogleCode();
                    return RedirectToAction("Register",new User());
                }
                if (await _userService.CreateUser(user))
                {
                    await CreateCookie(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("UserExists","This User already exists!");
                    GetGoogleCode();
                    return View("Register",new User());
                }
            }
            return View("Index");
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
           /* var clientId = _config.GetSection("Authentication").GetSection("Google")["ClientId"]; ;
            var url = "https://localhost:7211/user/signin-google";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;*/
           GetGoogleCode();
            return View(new User ());
        }
        public IActionResult Register()
        {
            GetGoogleCode();
            return View(new User());
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(User user)
        {
            if(await _userService.Login(user))
            {
                await CreateCookie(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Message"] = "Login failed! Please check your Email or Password!";
                return View("Login",new User());
            }
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home",new User());
        }
       /* [HttpGet("login-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var clientId = _config.GetSection("Authentication").GetSection("Google")["ClientId"]; ;
            var url = "https://localhost:7211/user/signin-google";
            var response = GoogleAuth.GetAuthUrl(clientId,url);
            ViewBag.response = response;
            return View();
        }*/
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLoginCallback(string code)
        {
            var clientId = _config.GetSection("Authentication").GetSection("Google")["ClientId"];
            var url = "https://localhost:7211/user/signin-google";
            var clientSecret = _config.GetSection("Authentication").GetSection("Google")["ClientSecret"];
            var token = await GoogleAuth.GetAuthAccessToken(code, clientId, clientSecret, url);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            JObject result = JObject.Parse(userProfile);
            var userEmail = result["email"];
            var userUsername = result["name"];

            User user = new User{Username = userUsername.ToString(), Email = userEmail.ToString(),Password = ""};
            if (!await _userService.CreateUser(user))
            {
                await _userService.Login(user);
            }
            await CreateCookie(user);

            return RedirectToAction("Index","Home");
        }
        private async Task CreateCookie(User user)
        {
            if(string.IsNullOrEmpty(user.Username))
            {
               user.Username = await _userService.GetUsername(user.Email);
            }
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, $"{user.Username}"),
            new Claim(ClaimTypes.Email,$"{user.Email}"),
            new Claim("UserRegisterStatus","Registered")
            };
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);                           
        }
        private async Task GetGoogleCode()
        {
            var clientId = _config.GetSection("Authentication").GetSection("Google")["ClientId"]; ;
            var url = "https://localhost:7211/user/signin-google";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;
        }
    }
}
