using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
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
        public IActionResult test(int code = 13)
        {
            ViewData["add_css"] = "account";
            return View("Login", new Tuple<string, int>("test123@gmail.com", code));
        }
        
        [HttpGet]
        public IActionResult Login(string ReturnUrl = "")
        {
            ViewData["add_css"] = "account";
            ViewData["returnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email = null, string ReturnUrl = "")
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
                
                StringBuilder sb = new StringBuilder();
                sb.Append("<!DOCTYPE html><html lang=\"en\"><head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" charset=\"UTF-8\"/> <title>Zermos</title> <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700\" rel=\"stylesheet\"/> <script src=\"https://kit.fontawesome.com/052f5fa8f8.js\" crossorigin=\"anonymous\"></script> <style>body{background: #f8f9fa;}.middle{ width: 100%; background-color: #ffffff; display: flex; flex-direction: column;}.zermos{font-family: 'Open Sans', sans-serif; font-weight: bold; font-size: 3rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.content{font-family: 'Open Sans', sans-serif; font-weight: 300; font-size: 1rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 400; font-size: 1rem; color: #344767; width: 100%; text-align: center; margin: 0;}.verify-button{font-family: 'Open Sans', sans-serif; font-weight: 600; font-size: 1.5rem; color: #ffffff; width: 50%; height: auto; background-color: #344767; border-radius: 14px; padding: 1rem; margin: 1rem 25%; border: none; cursor: pointer;}.verify-button:hover{background-color: #2c3e50;}.verify-button:active{background-color: #344767;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 200; font-size: 0.8rem; color: #344767; width: 100%; text-align: center; margin: 0; padding-bottom: 1rem;}</style></head><body><div class=\"middle\"> <h1 class=\"zermos\">Zermos</h1> <p class=\"content\">Je hebt zojuist een account aangemaakt bij <strong>Zermos</strong>. Daarom is er een e-mail verzonden om te verifiëren dat je daadwerkelijk een account wilt aanmaken.</p><p class=\"content\">Bevestig je e-mailadres door op de onderstaande knop te klikken. Deze stap voegt extra beveiliging toe aan je account door te verifiëren dat jij de eigenaar bent van dit e-mailadres.</p><a onclick=\"window.location.href='");
                sb.Append($"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountCreation?token={newUser.VerificationToken}&email={newUser.email}{(ReturnUrl == "" ? "" : "&returnUrl=" + ReturnUrl)}");
                sb.Append(";\" class=\"verify-button\">Log nu in!</a> <p class=\"valid-time\">P.S. deze link is 10 minuten geldig.</p></div></body></html>");

                mimeMessage = new MimeMessage();
                mimeMessage.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
                mimeMessage.To.Add(MailboxAddress.Parse(email));
                mimeMessage.Subject = "Test Email Subject";
                mimeMessage.Body = new TextPart(TextFormat.Html)
                {
                    Text = sb.ToString()
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
            
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("<!DOCTYPE html><html lang=\"en\"><head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" charset=\"UTF-8\"/> <title>Zermos</title> <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700\" rel=\"stylesheet\"/> <script src=\"https://kit.fontawesome.com/052f5fa8f8.js\" crossorigin=\"anonymous\"></script> <style>body{background: #f8f9fa;}.middle{ width: 100%; background-color: #ffffff; display: flex; flex-direction: column;}.zermos{font-family: 'Open Sans', sans-serif; font-weight: bold; font-size: 3rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.content{font-family: 'Open Sans', sans-serif; font-weight: 300; font-size: 1rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 400; font-size: 1rem; color: #344767; width: 100%; text-align: center; margin: 0;}.verify-button{font-family: 'Open Sans', sans-serif; font-weight: 600; font-size: 1.5rem; color: #ffffff; width: 50%; height: auto; background-color: #344767; border-radius: 14px; padding: 1rem; margin: 1rem 25%; border: none; cursor: pointer; text-decoration: none; text-align: center;}.verify-button:hover{background-color: #2c3e50;}.verify-button:active{background-color: #344767;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 200; font-size: 0.8rem; color: #344767; width: 100%; text-align: center; margin: 0; padding-bottom: 1rem;}</style></head><body><div class=\"middle\"> <h1 class=\"zermos\">Zermos</h1> <p class=\"content\">Je hebt zojuist een inlog e-mail aangevraagd om in te loggen bij <strong>Zermos</strong>. Daarom is er een e-mail verzonden om te verifiëren dat je daadwerkelijk een wilt inloggen.</p><a href='");
            sb2.Append($"{Request.Scheme}://{Request.Host}{Request.PathBase}/VerifyAccountLogin?token={user.VerificationToken}&email={user.email}{(ReturnUrl == "" ? "" : "&returnUrl=" + ReturnUrl)}");
            sb2.Append("' class=\"verify-button\">Log nu in!</a> <p class=\"valid-time\">P.S. deze link is 10 minuten geldig.</p></div></body></html>");

            mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = "Test Email Subject";
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = sb2.ToString()
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
        public async Task<IActionResult> VerifyAccountCreation(string token, string email, string ReturnUrl = "")
        {
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

            ViewData["add_css"] = "account";
            return await VerificationSuccess(email.ToLower(), ReturnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccountLogin(string token, string email, string returnUrl)
        {
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null) return VerificationFailed(1); //user not found

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
                return VerificationFailed(2); //token expired

            if (user.VerificationToken != token) return VerificationFailed(3); //invalid token

            user.VerificationToken = String.Empty;
            user.CreatedAt = DateTime.MinValue;
            await _users.UpdateUserAsync(email.ToLower(), user);
            ViewData["add_css"] = "account";
            return await VerificationSuccess(email.ToLower(), returnUrl);
        }

        [NonAction]
        private async Task<IActionResult> VerificationSuccess(string email , string ReturnUrl)
        {
            ViewData["add_css"] = "account";
            var claims = new List<Claim>
            {
                new Claim("email", email),
                new Claim("role", "user"),
            };
            
            var user = await _users.GetUserAsync(email);
            
            HttpContext.Response.Cookies.Append("theme", user.theme ?? "light");

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            if (string.IsNullOrEmpty(ReturnUrl))
            {
                return View("Login", new Tuple<string, int>(email, 13));
            }
            
            return Redirect(ReturnUrl);
            
        }

        [NonAction]
        private IActionResult VerificationFailed(int code)
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
            
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            
            return View("Login", new Tuple<string, int>(null, 3));
        }
    }
}