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
using WebApplication1.Services;
using WebApplication1.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApplication1.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IUrlService _urlService;
        public UserController(IUserService userService,IConfiguration config,IUrlService urlService)
        {
            _userService = userService;
            _config = config;
            _urlService = urlService;
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
            var userEmail = result["email"].ToString();
            var userUsername = result["name"].ToString();
            var userLastname = result["family_name"].ToString();
            var userName = result["given_name"].ToString();
            //var name

            User user = new User{Email = userEmail,Password = "",Name = userName,LastName = userLastname};
            if (!await _userService.CreateUser(user))
            {

                if(!await _userService.Login(user))
                {
                    ModelState.AddModelError("UserExists", "This User already exists!");
                    return RedirectToAction("Register");
                }
            }

            await ThirdPartyCreateCookie(user);

            return RedirectToAction("Index","Home");
        }
        private async Task CreateCookie(User user)
        {
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, $"{user.Name}"),
            new Claim(ClaimTypes.Email,$"{user.Email}"),
            new Claim("ThirdParty","False"),
            new Claim("UserRegisterStatus","Registered")
            };
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);                           
        }
        private async Task ThirdPartyCreateCookie(User user)
        {
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, $"{user.Name}"),
            new Claim(ClaimTypes.Email,$"{user.Email}"),
            new Claim("ThirdParty","True"),
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
        [HttpGet("log/{shortenedUrl}")]
        public async Task<IActionResult> Log(string shortenedUrl)
        {
            //I don't want other users to access log files that aren't theirs.
            if (User.Identity.IsAuthenticated && await _userService.GetUserByEmail(User.GetEmail())!= null)
            {
                var b = _urlService.GetClickInfo().ToList();
                var urlInfo = _urlService.GetLinks().FirstOrDefault(l => l.ShortenedUrl == shortenedUrl);
                return View("Log", urlInfo);
            }
            return BadRequest();
            
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Profile(User user)
        {
            if(User.Identity.IsAuthenticated)
            {
                return View(await _userService.GetUserByEmail(User.GetEmail()));
            }
            return StatusCode(401);
        }
		[HttpPost("profile")]
		public async Task<IActionResult> UpdateProfile(User user)
		{
			if (User.Identity.IsAuthenticated)
			{
				if(ModelState.IsValid || (User.IsThirdParty() && ModelState["Password"].RawValue == null && ModelState["ConfirmPassword"].RawValue == null))
                {
					 if(await _userService.CheckIfUserExists(user) && (user.Email != User.GetEmail()))
                     {
						  ModelState.AddModelError("UserExists", "This User already exists!");
                          return View("profile",user);
					 }
                    await HttpContext.SignOutAsync("CookieAuth");
                    if (User.IsThirdParty())
                    {
                        await _userService.UpdateUser(
                        User.GetEmail(),
                        user.Name,
                        user.LastName,
                        user.Email,
                        ""
                        );
                        await ThirdPartyCreateCookie(user);
                    }
                    else
                    {
                        await _userService.UpdateUser(
                         User.GetEmail(),
                         user.Name,
                         user.LastName,
                         user.Email,
                         user.Password
                         );
                        await CreateCookie(user);
                    }
                    
                }
			}
            return RedirectToAction("Index", "Home");
		}
	}
}
