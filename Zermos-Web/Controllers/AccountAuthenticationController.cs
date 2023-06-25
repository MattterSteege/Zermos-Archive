using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
    public class AccountAuthenticationController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AccountAuthenticationController> _logger;
        private readonly Users _users;

        public AccountAuthenticationController(ILogger<AccountAuthenticationController> logger, IConfiguration config, Users users)
        {
            _logger = logger;
            _config = config;
            _users = users;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            ViewData["add_css"] = "account";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email = null)
        {
            ViewData["add_css"] = "account";
            var user = await _users.GetUserAsync(email);
            MimeMessage mimeMessage;
            SmtpClient smtp;
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

                mimeMessage = new MimeMessage();
                mimeMessage.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
                mimeMessage.To.Add(MailboxAddress.Parse(email));
                mimeMessage.Subject = "Test Email Subject";
                mimeMessage.Body = new TextPart(TextFormat.Html)
                {
                    Text =
                        $"klik <a href=\"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountCreation?token={newUser.VerificationToken}&email={newUser.email}\">hier</a> om je Zermos account te verifiëren. Deze link is 10 minuten geldig."
                };

                // send email
                smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("Email:EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.GetSection("Email:EmailUsername").Value,
                    _config.GetSection("Email:EmailPassword").Value);
                await smtp.SendAsync(mimeMessage);
                smtp.Disconnect(true);

                return View(new Tuple<string, int>(email, 12));
            }

            //update user's verification token
            user.VerificationToken = TokenUtils.RandomString(32);
            user.CreatedAt = DateTime.Now;
            await _users.UpdateUserAsync(email, user);

            mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = "Test Email Subject";
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text =
                    $"klik <a href=\"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountLogin?token={user.VerificationToken}&email={user.email}\">hier</a> om in te loggen in je Zermos account. Deze link is 10 minuten geldig."
            };

            // send email
            smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("Email:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.GetSection("Email:EmailUsername").Value,
                _config.GetSection("Email:EmailPassword").Value);
            await smtp.SendAsync(mimeMessage);
            smtp.Disconnect(true);

            return View(new Tuple<string, int>(email, 11));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountCreation(string token, string email)
        {
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null) return await VerificationFailed(1); //user not found

            if (user.IsVerified) return await VerificationFailed(4); //user already verified

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
                return await VerificationFailed(2); //token expired

            if (user.VerificationToken != token) return await VerificationFailed(3); //token incorrect

            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = null;
            user.CreatedAt = null;
            await _users.UpdateUserAsync(email.ToLower(), user);

            ViewData["add_css"] = "account";
            return await VerificationSuccess(email.ToLower());
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountLogin(string token, string email)
        {
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null) return await VerificationFailed(1); //user not found

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
                return await VerificationFailed(2); //token expired

            if (user.VerificationToken != token) return await VerificationFailed(3); //invalid token

            user.VerificationToken = null;
            user.CreatedAt = null;
            await _users.UpdateUserAsync(email.ToLower(), user);
            ViewData["add_css"] = "account";
            return await VerificationSuccess(email.ToLower());
        }

        [NonAction]
        private async Task<IActionResult> VerificationSuccess(string email)
        {
            ViewData["add_css"] = "account";
            var claims = new List<Claim>
            {
                new Claim("email", email),
                new Claim("role", "user")
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            return View("Login", new Tuple<string, int>(email, 13));
        }

        [NonAction]
        private async Task<IActionResult> VerificationFailed(int code)
        {
            ViewData["add_css"] = "account";
            switch (code)
            {
                case 1:
                    return View("Login", new Tuple<string, int>(null, 21));
                case 2:
                    return View("Login", new Tuple<string, int>(null, 22));
                case 3:
                    return View("Login", new Tuple<string, int>(null, 23));
                case 4:
                    return View("Login", new Tuple<string, int>(null, 24));
                default:
                    return View("Login", new Tuple<string, int>(null, 4));
            }
        }


        public async Task<IActionResult> Logout()
        {
            ViewData["add_css"] = "account";
            await HttpContext.SignOutAsync();
            return View("Login", new Tuple<string, int>(null, 3));
        }
    }
}