using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly Users _users;
        
        public AccountController(IConfiguration configuration, Users users)
        {
            _config = configuration;
            _users = users;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            string json = await new StreamReader(Request.Body).ReadToEndAsync();
            var requestModel = JsonConvert.DeserializeObject<UserRegisterRequestModel>(json);

            var user = await _users.GetUserAsync(requestModel.Email);
            if (user == null || user.IsVerified == false)
            {
                await CreateNewUser(requestModel.Email);
                return Ok("User created!");
            }

            await VerifyUser(requestModel.Email);
            return Ok("User already exists!");
        }

        #region new user verification
        private async Task CreateNewUser(string requestModelEmail)
        {
            var newUser = new user
            {
                email = requestModelEmail,
                //generate random string for verification token
                VerificationToken = TokenUtils.RandomString(32),
                CreatedAt = DateTime.Now
            };

            await _users.AddUserAsync(newUser);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(requestModelEmail));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"klik <a href=\"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/Account/VerifyAccountCreation?token={newUser.VerificationToken}&email={newUser.email}\">hier</a> om je Zermos account te verifiëren. Deze link is 10 minuten geldig."
            };
            
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("Email:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.GetSection("Email:EmailUsername").Value, _config.GetSection("Email:EmailPassword").Value);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        
        [HttpGet("VerifyAccountCreation")]
        public async Task<IActionResult> Verify(string token, string email)
        {
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            if (user.VerificationToken == "done")
            {
                return BadRequest("User already verified!");
            }
            
            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
            {
                await CreateNewUser(email.ToLower());
                return BadRequest("Token expired, sending new token!");
            }

            if (user.VerificationToken != token)
            {
                await CreateNewUser(email.ToLower());
                return BadRequest("Invalid token, sending new token!");
            }

            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = null;
            user.CreatedAt = null;
            await _users.UpdateUserAsync(email.ToLower(), user);
            
            return Ok("User verified!");
        }
        #endregion
        
        #region verify existing user
        private async Task VerifyUser(string requestModelEmail)
        {
            //update user's verification token
            var user = await _users.GetUserAsync(requestModelEmail);
            user.VerificationToken = TokenUtils.RandomString(32);
            user.CreatedAt = DateTime.Now;
            await _users.UpdateUserAsync(requestModelEmail, user);
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(requestModelEmail));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"klik <a href=\"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/Account/VerifyAccountLogin?token={user.VerificationToken}&email={user.email}\">hier</a> om in te loggen in je Zermos account. Deze link is 10 minuten geldig."
            };
            
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("Email:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.GetSection("Email:EmailUsername").Value, _config.GetSection("Email:EmailPassword").Value);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        
        [HttpGet("VerifyAccountLogin")]
        public async Task VerifyLogin(string token, string email)
        {
            var user = await _users.GetUserAsync(email.ToLower());
            if (user == null)
            {
                throw new Exception("User not found!");
            }

            if (user.CreatedAt != null && user.CreatedAt.Value.AddMinutes(10) < DateTime.Now)
            {
                await VerifyUser(email.ToLower());
                throw new Exception("Token expired, sending new token!");
            }

            if (user.VerificationToken != token)
            {
                await VerifyUser(email.ToLower());
                throw new Exception("Invalid token, sending new token!");
            }

            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = null;
            user.CreatedAt = null;
            await _users.UpdateUserAsync(email.ToLower(), user);
        }


        #endregion
    }
}