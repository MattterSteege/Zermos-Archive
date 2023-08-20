using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
    public class AccountAuthenticationController : Controller
    {
        private readonly ILogger<AccountAuthenticationController> _logger;
        private readonly Users _users;

        public AccountAuthenticationController(ILogger<AccountAuthenticationController> logger, IConfiguration config, Users users)
        {
            _logger = logger;
            _users = users;
        }
        
        #if DEBUG
        [HttpGet]
        public IActionResult test(int code = 13)
        {
            return PartialView("Login", new Tuple<string, int>("test123@gmail.com", code));
        }
        #endif
        
        
        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email = null, string returnUrl = "", bool isPWA = false)
        {
            var user = await _users.GetUserAsync(email);
            
            Response.Cookies.Append("try_login", "true");
            
            if (user == null || user.IsVerified == false)
            {
                var newUser = new user
                {
                    email = email,
                    //generate random string for verification token
                    VerificationToken = TokenUtils.RandomString(32),
                    CreatedAt = DateTime.Now
                };

                await _users.AddUserAsync(newUser);
                
                await MailgunService.SendEmail(email, "Zermos - Bevestig je e-mailadres", $"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountCreation?token={newUser.VerificationToken}&email={newUser.email}{(returnUrl == "" ? "" : "&returnUrl=" + returnUrl)}&isPWA={(isPWA ? "true" : "false")}", true);

                return PartialView(new Tuple<string, int>(email, 12));
            }

            //update user's verification token
            user.VerificationToken = TokenUtils.RandomString(32);
            user.CreatedAt = DateTime.Now;
            await _users.UpdateUserAsync(email, user);
            
            await MailgunService.SendEmail(email, "Zermos - Bevestig je e-mailadres", $"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountLogin?token={user.VerificationToken}&email={user.email}{(returnUrl == "" ? "" : "&returnUrl=" + returnUrl)}&isPWA={(isPWA ? "true" : "false")}", true);

            return PartialView(new Tuple<string, int>(email, 11));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountCreation(string token, string email, string returnUrl = "", bool isPWA = false)
        {
            if (Request.Cookies.Count == 0)
            {
                return VerificationFailed(5);
            }
            
            if (isPWA)
            {
                return Redirect(
                    $"web+zermos:///VerifyAccountCreation?token={token}&email={email}{(returnUrl == "" ? "" : "&returnUrl=" + returnUrl)}");
            }
            
            //remove cookie "try_login"
            Response.Cookies.Delete("try_login");
            
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null) return VerificationFailed(1); //user not found

            if (user.IsVerified) return VerificationFailed(4); //user already verified

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
                return VerificationFailed(2); //token expired

            if (user.VerificationToken != token) return VerificationFailed(3); //token incorrect

            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = String.Empty;
            user.CreatedAt = DateTime.MinValue;
            await _users.UpdateUserAsync(email.ToLower(), user);
            
            return await VerificationSuccess(email.ToLower(), returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountLogin(string token, string email, string returnUrl, bool isPWA)
        {
            if (Request.Cookies.Count == 0)
            {
                return VerificationFailed(5);
            }

            if (isPWA)
            {
                return Redirect(
                    $"web+zermos:///VerifyAccountLogin?token={token}&email={email}{(returnUrl == "" ? "" : "&returnUrl=" + returnUrl)}");
            }
            
            //remove cookie "try_login"
            Response.Cookies.Delete("try_login");
            
            _logger.LogInformation("User {email} is trying to to log in", email);

            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null) return VerificationFailed(1); //user not found

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
                return VerificationFailed(2); //token expired

            if (user.VerificationToken != token) return VerificationFailed(3); //invalid token

            user.VerificationToken = String.Empty;
            user.CreatedAt = DateTime.MinValue;
            await _users.UpdateUserAsync(email.ToLower(), user);
            return await VerificationSuccess(email.ToLower(), returnUrl);
        }

        [NonAction]
        private async Task<IActionResult> VerificationSuccess(string email , string ReturnUrl)
        {
            var claims = new List<Claim>
            {
                new("email", email),
                new("role", "user"),
            };
            
            var user = await _users.GetUserAsync(email);
            
            HttpContext.Response.Cookies.Append("theme", user.theme ?? "light");

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            _logger.LogInformation("User {email} logged in", email);
            
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                return PartialView("Login", new Tuple<string, int>(email, 13));
            }
            
            return Redirect(ReturnUrl);
            
        }

        [NonAction]
        private IActionResult VerificationFailed(int code)
        {
            switch (code)
            {
                case 1:
                    _logger.LogWarning("Verification failed with code {code} (User not found in database)", 21);
                    return PartialView("Login", new Tuple<string, int>(null, 21));
                case 2:
                    _logger.LogWarning("Verification failed with code {code} (Token expired)", 22);
                    return PartialView("Login", new Tuple<string, int>(null, 22));
                case 3:
                    _logger.LogWarning("Verification failed with code {code} (Token incorrect)", 23);
                    return PartialView("Login", new Tuple<string, int>(null, 23));
                case 4:
                    _logger.LogWarning("Verification failed with code {code} (User already verified)", 24);
                    return PartialView("Login", new Tuple<string, int>(null, 24));
                default:
                    _logger.LogWarning("Verification failed with code {code} (Unknown error)", 4);
                    return PartialView("Login", new Tuple<string, int>(null, 4));
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            
            return PartialView("Login", new Tuple<string, int>(null, 3));
        }
    }
}