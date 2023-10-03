using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers;

public class AuthenticationController : Controller
{
    private readonly Users _users;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(Users users, ILogger<AuthenticationController> logger)
    {
        _users = users;
        _logger = logger;
    }
#if RELEASE
    const string redirectUrl = "https://zermos.kronk.tech/Login/Callback";
#else
    const string redirectUrl = "https://192.168.178.34:5001/Login/Callback";
#endif
    const string clientId = "REDACTED_MS_CLIENT_ID";
    const string clientSecret = "lcV8Q~GbQjBv45fivMgN3ARP~UHPNSuV259gQcU7";

    [ZermosPage]
    [Route("/Login")]
    public IActionResult Teams()
    {
        string redirect = "https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize?client_id=" +
                          clientId + "&response_type=code&redirect_uri=" + redirectUrl +
                          "&response_mode=query&scope=User.Read&state=" + TokenUtils.RandomString();
        return PartialView("Login", new loginModel {email = redirect, code = 0});
    }

    [ZermosPage]
    [Route("/Login/Callback")]
    public async Task<IActionResult> TeamsCallback(string code, string state, string session_state)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://login.microsoftonline.com/organizations/oauth2/v2.0/token");
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("client_id", clientId));
        collection.Add(new("scope", "User.Read"));
        collection.Add(new("code", code));
        collection.Add(new("redirect_uri", redirectUrl));
        collection.Add(new("grant_type", "authorization_code"));
        collection.Add(new("client_secret", clientSecret));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return VerificationFailed(4);
        }

        var responseString = await response.Content.ReadAsStringAsync();

        var auth = JsonConvert.DeserializeObject<TeamsAuthenticationModel>(responseString);

        request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer " + auth.access_token);

        response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            return VerificationFailed(23);
        }

        var teamsUser = JsonConvert.DeserializeObject<TeamsUserModel>(await response.Content.ReadAsStringAsync());
        
        return await VerificationSuccess(teamsUser.mail, null);
    }

    [NonAction]
    private async Task<IActionResult> VerificationSuccess(string email, string ReturnUrl, bool OneSessionLogin = false)
    {
        var claims = new List<Claim>
        {
            new("email", email),
            new("role", "user"),
        };

        var user = await _users.GetUserAsync(email);
        
        if (user == null)
        {
            user = new user
            {
                email = email,
                theme = "light"
            };
            await _users.AddUserAsync(user);
        }

        HttpContext.Response.Cookies.Append("theme", user.theme ?? "light");

        await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

        _logger.LogInformation("User {email} logged in", email);

        if (string.IsNullOrEmpty(ReturnUrl))
        {
            return PartialView("Login", new loginModel {email = email, code = 13});
        }

        return Redirect(ReturnUrl);
    }

    [NonAction]
    private IActionResult VerificationFailed(int code)
    {
        switch (code)
        {
            case 21:
                _logger.LogWarning("Verification failed with code {code} (User not found in database)", 21);
                return PartialView("Login", new loginModel {code = 21});
            case 22:
                _logger.LogWarning("Verification failed with code {code} (Token expired)", 22);
                return PartialView("Login", new loginModel {code = 22});
            case 23:
                _logger.LogWarning("Verification failed with code {code} (Token incorrect)", 23);
                return PartialView("Login", new loginModel {code = 23});
            case 24:
                _logger.LogWarning("Verification failed with code {code} (User already verified)", 24);
                return PartialView("Login", new loginModel {code = 24});
            default:
                _logger.LogWarning("Verification failed with code {code} (Unknown error)", 4);
                return PartialView("Login", new loginModel {code = 4});
        }
    }

    [ZermosPage]
    [Route("/Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _logger.LogInformation("User {email} logged out", User.FindFirst("email")?.Value);
        return PartialView("Login", new loginModel {code = 3});
    }

    #if DEBUG
    [ZermosPage]
    [Route("/LoginPage")]
    public async Task<IActionResult> TestPages(int code = 0)
    {
        return PartialView("Login", new loginModel {code = code});
    }
    #endif

    public class loginModel
    {
        public string email { get; set; }
        public int code { get; set; }
    }
}